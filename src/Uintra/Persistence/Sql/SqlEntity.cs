﻿namespace Uintra.Persistence.Sql
{
    public abstract class SqlEntity<TKey>
    {
        public abstract TKey Id { get; set; }
    }
}