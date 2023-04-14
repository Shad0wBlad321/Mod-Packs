using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BillDoorsFramework
{
    [DefOf]
    public static class ShaderTypeDefOf
    {
        static ShaderTypeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ShaderTypeDefOf));
        }
        public static ShaderTypeDef TransparentPostLight;
    }
}
