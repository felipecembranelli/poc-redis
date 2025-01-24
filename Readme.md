# Redis Studying Documentation

## Main References

Setup local Redis
https://phoenixnap.com/kb/docker-redis

Using Redis Hash to deal with collections: 
https://medium.com/@danilosilva_37526/using-redis-hash-to-deal-with-collections-569449ac0384

Redis for .NET Developer – Redis Hash Datatype
https://taswar.zeytinsoft.com/redis-hash-datatype/

How to add a basic API Cache to your ASP.NET Core application
https://redis.io/learn/develop/dotnet/aspnetcore/caching/basic-api-caching

Distributed caching in ASP.NET Core
https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-9.0

## Eviction Policies

https://redis.io/blog/cache-eviction-strategies/

## Redis Key Naming Best Practices
https://medium.com/nerd-for-tech/unveiling-the-art-of-redis-key-naming-best-practices-6e20f3839e4a

### 1. Introduction

When working with Redis, it's important to follow best practices for naming keys to ensure consistency, readability, and maintainability. Here are some key guidelines to consider:

#### 1.1 Use a Consistent Naming Convention
- **Use Namespaces**: Use colons (`:`) to create namespaces. For example, `user:1000`, `user:1000:profile`, `user:1000:settings`.
- **Descriptive Names**: Use descriptive names that clearly indicate the purpose of the key. Avoid abbreviations that are not commonly understood.
- **Avoid Special Characters**: Stick to alphanumeric characters and colons. Avoid using special characters like spaces, slashes, or backslashes.

#### 1.2 Keep Key Names Short
- **Efficiency**: Shorter key names reduce memory usage and improve performance.
- **Readability**: Ensure that key names are still readable and meaningful even when kept short.

#### 1.3 Use Consistent Case
- **Lowercase**: Prefer using lowercase letters for key names to maintain consistency and avoid case sensitivity issues.

#### 1.4 Include Versioning
- **Versioning**: If your data structure might change over time, include a version number in the key name. For example, `user:1000:v1`, `user:1000:v2`.

#### 1.5 Avoid Key Collisions
- **Unique Identifiers**: Ensure that key names are unique to avoid collisions. Use unique identifiers like user IDs, timestamps, or UUIDs.

#### 1.6 Use Meaningful Prefixes
- **Prefixes**: Use meaningful prefixes to group related keys together. For example, `session:`, `cache:`, `config:`.

#### 1.7 Document Key Naming Conventions
- **Documentation**: Maintain documentation of your key naming conventions to ensure that all team members follow the same guidelines.

By following these best practices, you can create a well-organized and efficient Redis keyspace that is easy to manage and understand.
