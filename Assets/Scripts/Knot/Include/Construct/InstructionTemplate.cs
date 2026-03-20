using System;
using System.Collections;
using Knot.Include.Construct;

namespace Knot.Include.Construct
{
    #region Param
    [Serializable]
    public class _InstrParamTemplate_: InstrParam
    {
        #region "These Properties MUSTn't Be Changed!"

        [DisplayInEditor("指令名称", Order = -100)]
        public override string Name { get; protected set; } = nameof(_InstrParamTemplate_);

        protected override string _ExecutorType { get; set; } = nameof(_InstrExecuteTemplate_);

        #endregion

        #region Const Implemented Property

        [DisplayInEditor("描述", Order = -80)]
        public override string Description { get; protected set; } = "_InstrParamTemplate_ Description";

        [DisplayInEditor("允许共存", Order = 10, Tooltip = "同一帧中是否允许存在多个相同类型的指令")]
        public override bool IsCanCoexist { get;  set; } = false;

        #endregion

        #region Basic Property

        [DisplayInEditor("是否在此处执行后释放", Order = 20)]
        public override bool IsRelease { get; set; } = false;

        [DisplayInEditor("可跳过", Order = 30, Tooltip = "该指令是否可以被跳过")]
        public override bool IsCanBeSkipped { get; set; } = true;

        [DisplayInEditor("需要等待", Order = 40, Tooltip = "该指令是否需要等待执行完成")]
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
        // Init will be auto called,so it's not necessary to call Init in this part.
        protected override void Init()
        {

        }

        // Execute will be called before CoExecute if there exists contents here.
        protected override void Execute()
        {

        }

        // CoExecute can be ignored if not needed, or auto called.
        protected override IEnumerator CoExecute()
        {

            yield return null;
        }

        // When executing this Instr, Interrupt it.
        protected override void Interrupt()
        {

        }

        // When this Instr is completed, End this.
        protected override void End()
        {

        }
        #endregion
    }
    #endregion
}
