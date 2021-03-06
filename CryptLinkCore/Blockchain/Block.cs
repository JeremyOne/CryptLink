﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace CryptLink {
	public class Block<T> : Hashable
        where T : Hashable {

		public Hash ParentHash { get; set; }
		public Hash GeneratorAddress { get; set; }
		public DateTime GenerationDate { get; set; }
		public long BlockNumber { get; set; }
		public List<T> BlockItems { get; set; }
        
        public bool Verify(Block<T> Parent){
			throw new NotImplementedException();
		}

        public override byte[] GetHashableData() {
            throw new NotImplementedException();
        }

    }
}

