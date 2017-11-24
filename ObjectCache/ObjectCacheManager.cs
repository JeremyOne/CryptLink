using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using System.Collections.Concurrent;

namespace CryptLink {
    class ObjectCacheManager {
        //public ConcurrentDictionary<Type, ObjectCache> CacheTypes = new ConcurrentDictionary<Type, ObjectCache>();

        ///// <summary>
        ///// The number of bytes to target for the total process memory size, in bytes
        ///// </summary>
        //public long TargetProcessMemorySize { get; set; }
        //public long TotalMemoryObjectCount { get; private set; }
        
        //public long MemoryObjectMaxSize { get; set; }
        //public long DatabaseObjectMaxSize { get; set; }
        //public long DiskObjectMaxSize { get; set; }

        //public TimeSpan DiskAutoSaveTrigger { get; set; }

        //public DateTime ManagedAt { get; set; }
        //public int ManageOperationCountTrigger { get; set; }
        //public int ManagedOperations { get; set; }
        //public TimeSpan ManageTimeTrigger { get; set; }

        //public string DatabaseFolder { get; set; }

        

        //public bool Add<T>(Hash Key, T Value, TimeSpan ExpireSpan) where T : Hashable {
        //    CacheItem i = new CacheItem() {
        //        AddedDate = DateTime.Now,
        //        Item = Value,
        //        ExpireDate = DateTime.Now.Add(ExpireSpan)
        //    };

        //    var cache = GetCacheForType(typeof(T));
        //    if (cache.Cache.ContainsKey(Key.Bytes)) {
        //        return false;
        //    }

            

        //}

        //public T Get<T>(Hash Key) where T : Hashable {

        //}

        //public T Exists<T>(Hash Key) where T : Hashable {

        //}

        //public bool Update<T>(Hash Key, T Value, TimeSpan ExpireSpan) where T : Hashable {

        //}

        //public bool Remove<T>(Hash Key) {

        //}

        //private ObjectCache GetCacheForType(Type T) {
        //    if (CacheTypes.ContainsKey(T)) {
        //        return CacheTypes[T];
        //    } else {
        //        //cache does not exist create it
        //        var newCache = new ObjectCache() {
        //            StorageDatabase = new LiteDatabase(TypeDatabasePath(T))
        //        };
        //    }
        //}

        //private string TypeDatabasePath(Type ForType) {
        //    return DatabaseFolder + System.IO.Path.DirectorySeparatorChar + ForType.FullName + ".db";
        //}

        ///// <summary>
        ///// Runs ManageNow() async if management is needed
        ///// </summary>
        //public void ManageCheckAsync() {
            
        //}

        //public void ManageNow() {
        //    //check total GC footprint
        //    //remove expired items
        //    //move low hit items to database if needed
        //}

        //public void SaveAll(TimeSpan ItemExpiryCutoff) {
        //    throw new NotImplementedException();
        //}

        //public void LoadAll() {
        //    throw new NotImplementedException();
        //}

        //public void Dispose() {
        //    throw new NotImplementedException();
        //}       

    }
}
