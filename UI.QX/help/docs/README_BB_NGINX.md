# NX.Node - NginX bumble bee

This is the code for the nginx bumble bee:
```JavaScript
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Engine.Hive;
using NX.Shared;

namespace Proc.NginX
{
    /// <summary>
    /// 
    /// NginX interface
    /// 
    /// </summary>
    public class ManagerClass : BumbleBeeClass
    {
        #region Constructor
        public ManagerClass(EnvironmentClass env)
            : base(env, "nginx")
        {
            // Handle the events
            this.AvailabilityChanged += delegate (bool isavailable)
            {
                // Check
                this.MakeConfig(isavailable && this.IsQueen);
            };

            // Track queen changes
            this.QueenChanged += delegate (bool isqueen)
            {
                // Check
                this.MakeConfig(this.IsAvailable && isqueen);
            };

            // Link for DNA changes
            this.Parent.Hive.Roster.DNAChanged += delegate (string dna, List<string> urls)
            {
                // Valid?
                if (this.ValidDNA(dna))
                {
                    // Update
                    this.UpdateProcess(dna, urls);
                    // And make config
                    this.MakeConfig(this.IsAvailable && this.IsQueen);
                }
            };

            // Get a list of all DNAs
            List<string> c_DNA = this.Parent.Hive.Roster.GetDNAs();
            // Loop thru
            foreach (string sDNA in c_DNA)
            {
                // Valid?
                if (this.ValidDNA(sDNA))
                {
                    // Update
                    this.UpdateProcess(sDNA, this.Parent.Hive.Roster.GetLocationsForDNA(sDNA));
                }
            }

            // Bootstap
            this.CheckForAvailability();
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// A map of processors vs. URL list
        /// 
        /// </summary>
        private NamedListClass<List<string>> Map { get; set; } = new NamedListClass<List<string>>();
        #endregion

        #region Methods
            ,,, CODE OMITTED.  SEE ProcNginX.Manager.cs
        #endregion
    }
}```

This very different than the [Redis](README_BB_REDIS.md) bumble bee.  Let's disect it.

The  property:

We start out by not having  client at all, as there is no client for NginX. We do have:
```JavaScript
private NamedListClass<List<string>> Map { get; set; } = new NamedListClass<List<string>>();
```
which is used by the code.

Now let's look at the constructor code.  There are multiple parts.

The first part is setting up the event delegate that is called when the container starts running
or when it stops.  Same as the redis bumble bee:
```JavaScript
this.AvailabilityChanged += delegate (bool isavailable)
{
    // Check
    this.MakeConfig(isavailable && this.IsQueen);
};
```
Rather than checking, we see if the genome is available and if we are the queen.  This is because 
the NginX controller should only run in one bee, and what better bee than the queen.

Next:
```JavaScript
this.QueenChanged += delegate (bool isqueen)
{
    // Check
    this.MakeConfig(this.IsAvailable && isqueen);
};
```
Not only can the availability of the genome change, but also our status as the queen.  If
another bee is elevated and we relinquesh the status, we are not longer in charge of the 
NginX configuration.

Next:
```JavaScript
this.Parent.Hive.Roster.DNAChanged += delegate (string dna, List<string> urls)
{
    // Valid?
    if (this.ValidDNA(dna))
    {
        // Update
        this.UpdateProcess(dna, urls);
        // And make config
        this.MakeConfig(this.IsAvailable && this.IsQueen);
    }
};
```
Here we track changes in bees, as they are added or removed. his is important as the
NginX configuration must match the bees in the hive.  

Next:
```JavaScript
 List<string> c_DNA = this.Parent.Hive.Roster.GetDNAs();
// Loop thru
foreach (string sDNA in c_DNA)
{
    // Valid?
    if (this.ValidDNA(sDNA))
    {
        // Update
        this.UpdateProcess(sDNA, this.Parent.Hive.Roster.GetLocationsForDNA(sDNA));
    }
}
```
This is where we build the initial map of the bees, grouped by DNA.

And last:
The last piece of constructor code is:
```JavaScript
this.CheckForAvailability();
```
Same as the redis bumble bee, we need to kickstart the process.

[Back to top](/help/docs/README.md)
