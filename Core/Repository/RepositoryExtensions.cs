﻿using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using SharpRepository.Repository;

namespace Core.Repository
{
    public static class RepositoryExtensions
    { 
        public static IEnumerable<T> LoadAll<T>(this IRepository<T> rs) where T : BaseEntity
        {
            return rs.AsQueryable().AsEnumerable();
        }

        public static IEnumerable<T> Where<T>(this IRepository<T> rs, Func<T, bool> predicate) where T : BaseEntity
        {
            return rs.AsQueryable().Where(predicate);
        }

        public static T GetSingle<T>(this IRepository<T> rs, Func<T, bool> predicate) where T : BaseEntity
        {
            return rs.AsQueryable().SingleOrDefault(predicate);
        }

        public static T GetFirst<T>(this IRepository<T> rs, Func<T, bool> predicate) where T : BaseEntity
        {
            return rs.AsQueryable().FirstOrDefault(predicate);
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> items, int chunkSize = 100)
        {
            if (chunkSize < 1)
            {
                throw new ArgumentException("Chunks should not be smaller than 1 element");
            }
            var status = new Status { EndOfSequence = false };
            using (var enumerator = items.GetEnumerator())
            {
                while (!status.EndOfSequence)
                {
                    yield return TakeOnEnumerator(enumerator, chunkSize, status);
                }
            }
        }

        private static IEnumerable<T> TakeOnEnumerator<T>(IEnumerator<T> enumerator, int count, Status status)
        {
            while (--count > 0 && (enumerator.MoveNext() || !(status.EndOfSequence = true)))
            {
                yield return enumerator.Current;
            }
        }

        private class Status
        {
            public bool EndOfSequence;
        }
          
        public static void Delete<T>(this IRepository<T> rs, Guid id) where T : BaseEntity
        {
            T entityToDelete = rs.AsQueryable().FirstOrDefault(x=> x.Id == id);
            if (entityToDelete != null)
            {
                rs.Delete(entityToDelete);
            }
        }
          
    }
}
