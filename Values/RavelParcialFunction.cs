﻿namespace Ravel.Values
{
    public class RavelParcialFunction : RavelFunction
    {
        public RavelParcialFunction(RavelFunction ravelFunction, RavelObject[] objs, RavelType type)
            : base(type, ravelFunction.RealParameters.Length - objs.Length)
        {
            RavelFunction = ravelFunction;
            Objs = objs;
        }

        public override bool IsConst => false;//?

        private RavelFunction RavelFunction { get; }
        private RavelObject[] Objs { get; }

        protected override void InvokeMust(NeoEvaluator evaluator, RavelObject[] obj)
        {
            RavelFunction.Invoke(evaluator, Objs.Concat(obj).ToArray());
        }
    }
}
