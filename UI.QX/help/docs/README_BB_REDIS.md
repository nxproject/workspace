# NX.Node - Redis bumble bee

This is the code for the redis bumble bee:
```JavaScript
using NX.Shared;
using StackExchange.Redis;

namespace NX.Engine.BumbleBees.Redis
{
    public class ManagerClass : BumbleBeeClass
    {
        #region Constructor
        public ManagerClass(EnvironmentClass env)
            : base(env, "redis")
        {
            //
            this.AvailabilityChanged += delegate (bool isavailable)
            {
                // Remove
                if(this.DB != null)
                {
                    this.DB = null;
                }

                if (this.Client != null)
                {
                    this.Client.Dispose();
                    this.Client = null;
                }
                
                //
                if (isavailable)
                {
                    //
                    ConfigurationOptions c_Cfg = ConfigurationOptions.Parse(this.Location.RemoveProtocol());
                    this.Client = ConnectionMultiplexer.Connect(c_Cfg);

                    this.DB = this.Client.GetDatabase();
                }
            };

            // Bootstap
            this.CheckForAvailability();
        }
        #endregion

        #region Redis
        /// <summary>
        /// 
        /// The client
        /// 
        /// </summary>
        public ConnectionMultiplexer Client { get; private set; }

        /// <summary>
        /// 
        /// The database
        /// 
        /// </summary>
        public IDatabase DB { get; private set; }
        #endregion
    }
}
```

This the format for bumble bees that are a client to a server container.  Let's disect it.

The easy part first, the two properties:
```JavaScript
public ConnectionMultiplexer Client { get; private set; }
```
This is where the **StackExchange.Redis.ConnectionMultiplexer** is kept.  This is the client.

```JavaScript
public IDatabase DB { get; private set; }
```
This is where the database itself is kept.

Now let's look at the constructor code.  There are two parts.

The first is setting up an event delegate that is called when the container starts running
or when it stops.
```JavaScript
this.AvailabilityChanged += delegate (bool isavailable)
{
    // Remove
    if(this.DB != null)
    {
        this.DB = null;
    }

    if (this.Client != null)
    {
        this.Client.Dispose();
        this.Client = null;
    }
                
    //
    if (isavailable)
    {
        //
        ConfigurationOptions c_Cfg = ConfigurationOptions.Parse(this.Location.RemoveProtocol());
        this.Client = ConnectionMultiplexer.Connect(c_Cfg);

        this.DB = this.Client.GetDatabase();
    }
};
```
In both cases, we remove the client and database.  Then if the container is available (running),
we setup a new client and database objects.

This means that the system will create and destroy the links whenever there isa container state
change.  The client and database are available to callers, as well as an **IsAvailable** boolean 
that allows the aller to decide what to do, depending on the state.

The last piece of constructor code is:
```JavaScript
this.CheckForAvailability();
```
While the event delegate will be called when there is a state change, this kickstarts the
process by doing an event call right away, to set the starting mode.

[Back to top](/help/docs/README.md)
