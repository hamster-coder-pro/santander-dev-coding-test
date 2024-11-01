# Santander - Developer Coding Test

## Description

Using ASP.NET Core, implement a RESTful API to retrieve the details of the first n "best stories" from the Hacker News API, where n is specified by the caller to the API.  
1. The Hacker News API is documented here: https://github.com/HackerNews/API .  
2. The IDs for the "best stories" can be retrieved from this URI: https://hacker-news.firebaseio.com/v0/beststories.json .  
3. The details for an individual story ID can be retrieved from this URI: https://hacker-news.firebaseio.com/v0/item/21233041.json (in this case for the story with ID 21233041)  
4. The API should return an array of the first n "best stories" as returned by the Hacker News API, sorted by their score in a descending order, in the form:  
#### In addition to the above,   
5. your API should be able to efficiently service large numbers of requests without risking overloading of the Hacker News API.  
6. You should share a public repository with us,  
7. that should include a README.md file which describes how to run the application, any assumptions you have made, and any enhancements or changes you would make, given the time.  


## Analysis, assumptions and limitation
After reviewing required Hacker News API actions I have composed a list of contraints:
1. Best stories list and details are changed very often  
2. Best stories (ids) are stored in descending score order.  
3. If best stories list ramain the same, it does not guaratee that details are not changed (scores could be changed or amount of comments.. etc..)
4. There is no information related to recently changed items
5. No notifications about the changes as well (web sockets or so...)

## Thoughts

It obvious, that to decrease amount of call to external system - our web api should have some kind of the local data copy.
It could be db, memory, some kind of distributed cache... whatever.

The main question is how aggregate stored data and what benefits this kind of aggregation has.
Any kind of cache will potentially make data "not actual" for the lifetime of cache object.
Angan, there are no notification about items are updated. So cache evection logic could not be implemented.

I thougts about mainly three types cache data stuctures.
1. Simple response cache (acheivable by using app.UseResponseCaching();)
   Potentially too much of the data stored (which is not needed with given amount of data)

2. Cache best stories ids response and than each of item response and retreive them by keys depenens on 'limit' records requested
   Slightly complex because of the logic of retrieving non cached items and in the same time deny of making the same requests to the parallel processes.
   But the main issue is possible inconsistance of the retreived data (use case):
   ```
   First request is /beststories?limit=5
     -- best items retieved and cached
	 -- 5 first items retrieved and chached
   Second request is /beststories?limit=10
	-- best items retireived from the cache
    -- 5 first items retrieved from the cahce
    -- 6-10 items terieved from the Hacker News API, !!!BUT!!! diring this time item 6-10 scores could become any even higher than 1st item therefore rule is broken because we are storing outdated info for best items ids and already retrieved items.
   ```

3. Becase of we have only up to 500 small size items. it spossible to store full last actual graph of ordered items. And this is a way I've implemented.
   Use Case:
   ```
   First request is /beststories?limit=10
     -- best items ids are retieved from Hacker News API
     -- first 10 items retieved from Hacker News API
     -- values conveted to output model (array of output items) and stored in cache.
   Second request is /beststories?limit=5
     -- retrive response from cache (it contains 10 records)
     -- we need only 5... so take first 5 records only and return as result;
   Third request is /beststories?limit=20
     -- retrive response from cache (it contains 10 records)
     -- amount of items less than we store in cache therefore we need to fully update the cache graph (so no phantom data appreared like in #2 cache case)
     -- best items ids are retieved from Hacker News API
     -- first 20 items retieved from Hacker News API
     -- values conveted to output model (array of output items) and stored in cache.
   4-N and so on...
   ``` 

For demo reasons I made cache item lifetime = 1 minute. which could 

## How to run the project...

1. From the Visual Studio  
*require .NET 8 SDK*
```
Steps are simple as usual... 
-- open solution in VS
-- F5 (run) the project
```


2. Build and run docker image :)
*should be executed from solution folder*

**Build docker image**
```
docker build . -t santander-test-web -f Test.Web\Dockerfile
```

**Run docker image**
```
docker run --name santander-test-web -d -p 35000:8080 -e ASPNETCORE_ENVIRONMENT=Development santander-test-web
```

**Clean Up**
```
docker rm santander-test-web -f
docker rmi santander-test-web -f
```

## Possible future enhansements..

- exception handling middleware
- logging
- performance metrics
- desributed cache usage in case of multi node deployments
- integration and unit tests in case of long term support and extending the system.
- analysys of incoming requests, and optimization cached aggregates depends on commonly used API requests