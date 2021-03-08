//--------------------------------------------------------------------------------

/// Packet Manager Requirements
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// Install-Package MongoDb.Driver -Version 2.11.0
/// Install-Package MongoDb.Bson -Version 2.11.0
///  

using Newtonsoft.Json.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

using NX.Shared;

namespace Proc.AO
{
    public class ExtraClass : ChildOfClass<Proc.AO.DatasetClass>
    {
        #region Constructor
        internal ExtraClass(Proc.AO.DatasetClass ds, string id)
            : base(ds)
        {
            // Assure #
            if (!id.StartsWith("#")) id = "#" + id;
            // Save
            this.Name = id;

            //  Get
            this.Object = new ObjectClass(this.Parent, this.Name);
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The name
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// The underlying object
        /// 
        /// </summary>
        internal ObjectClass Object { get; set; }
        #endregion
    }
}