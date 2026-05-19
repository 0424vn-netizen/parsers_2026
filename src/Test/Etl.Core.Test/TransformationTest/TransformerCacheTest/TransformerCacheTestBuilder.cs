using Etl.Core.Extraction;
using Etl.Core.Transformation;
using Etl.Core.Transformation.Fields;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Core.Test.TransformationTest.TransformerCacheTest
{
    //public class TransformerCacheTestBuilder
    //{
    //    private readonly Transformer _transformDef;
    //    private readonly Layout _layout;

    //    public TransformerCacheTestBuilder()
    //    {
    //        _transformDef = new Transformer();  
    //        _layout = new Layout(); 
    //    }

    //    public TransformerCache Build()
    //    {
    //        return new TransformerCache(_transformDef,_layout);
    //    }
    //}


    public class TransformerCacheTestBuilder
    {
        private readonly ServiceCollection _instance;

        public TransformerCacheTestBuilder()
        {
            _instance = new ServiceCollection();
        }

        public IServiceCollection Build()
        {
            return _instance;
        }
    }
}
