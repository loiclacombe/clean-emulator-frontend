//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CleanEmulatorFrontend.SqLiteCache
{
    using System;
    using System.Collections.Generic;
    
    public partial class game
    {
        public long game_id { get; set; }
        public System.Guid gguid { get; set; }
        public long played { get; set; }
        public string description { get; set; }
        public string launch_path { get; set; }
        public long emusys_id { get; set; }
    
        public virtual emulated_system emulated_system { get; set; }
        public virtual game_base_path game_base_path { get; set; }
        public virtual last_played last_played { get; set; }
    }
}