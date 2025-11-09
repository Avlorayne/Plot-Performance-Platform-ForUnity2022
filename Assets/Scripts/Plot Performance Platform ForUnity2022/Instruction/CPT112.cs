namespace Plot_Performance_Platform_ForUnity2022.Instruction
{
    public class CPT112Param: InstrParam
    {
        public override string Name { get; set; } = "CPT112Param";
        public override string Description { get; set; } = "CPT112Param Description";
        public override int CoexistingQuantity { get; set; } = 1;
    }

    public class CPT112Execute: InstrExecute
    {
        // 执行指令
        public override void Exeute()
        {

        }
    }

}
