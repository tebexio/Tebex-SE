using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TebexSE.Commands
{
    public abstract class TebexCommandModule
    {
        public abstract void HandleResponse(JObject response);

        public abstract void HandleError(Exception e);
    }
}
