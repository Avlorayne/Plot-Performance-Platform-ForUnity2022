using System;
using System.Collections;
using UnityEngine;
using Plot_Performance_Platform_ForUnity2022.Include.Construct;
using Plot_Performance_Platform_ForUnity2022.src.Allocate;

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

        #region Const Implemented Property
        public override string Description { get; protected set; } = "_InstrParamTemplate_ Description";
        public override bool IsCanCoexist { get;  set; } = false;
        #endregion

        #region Basic Property
        public override bool IsRelese { get; set; } = false;
        public override bool IsCanBeSkipped { get; set; } = true;
        public override bool IsBeWaited { get; set; } = false;

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

        protected override void Init()
        {

        }

        /// 执行指令
        protected override void Execute()
        {

        }


        protected override IEnumerator CoExecute()
        {

            yield return null;
        }

        // 中断指令
        protected override void Interrupt()
        {

        }

        // 结束指令
        protected override void End()
        {

        }
        #endregion
    }
    #endregion
}
