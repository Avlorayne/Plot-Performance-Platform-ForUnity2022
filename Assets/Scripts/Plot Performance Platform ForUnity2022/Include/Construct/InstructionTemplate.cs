using System;
using System.Collections;
using UnityEngine;
using Plot_Performance_Platform_ForUnity2022.Include.Construct;

namespace Plot_Performance_Platform_ForUnity2022.Instruction
{
    #region Param
    [Serializable]
    public class _InstrParamTemplate_: InstrParam
    {
        #region "These Properties MUSTn't Be Changed!"
        public override string Name { get; protected set; } = nameof(_InstrParamTemplate_);
        protected override string _ExecutorType { get; set; } = nameof(_InstrExecuteTemplate_);
        #endregion

        #region Const Intended Property
        public override string Description { get; protected set; } = "_InstrParamTemplate_ Description";
        public override bool IsCanCoexist { get;  set; } = false;
        #endregion

        #region Basic Property
        public override bool IsRelese { get; set; } = false;
        public override bool IsCanBeSkipped { get; set; } = false;
        #endregion
    }
    #endregion

    #region Executor
    public class _InstrExecuteTemplate_: InstrExecute
    {
        #region Execute Param



        #endregion

        #region Executor

        void Start()
        {

        }

        void Update()
        {

        }

        public override void Execute()
        {

        }

        public override void Interrupt()
        {

        }
        #endregion
    }
    #endregion
}
