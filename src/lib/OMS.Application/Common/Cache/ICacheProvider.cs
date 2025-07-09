﻿namespace OMS.Application.Abstractions.Cache;

public interface ICacheProvider
{
    Task<T> GetAsync<T>(string key);
    
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    
    Task RemoveAsync(string key);
}