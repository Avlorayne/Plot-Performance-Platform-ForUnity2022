namespace Plot_Performance_Platform_ForUnity2022.Instruction
{
public class CPTParam: InstrParam
{
    public override string Name { get; set; } = "CPTParam";
    public override string Description { get; set; } = "CPTParam Description";
    public override int CoexistingQuantity { get; set; } = 1;

    public string usi = "isaasjda";
}

public class CPTExecute: InstrExecute
{
    // 执行指令
    public override void Exeute()
    {

    }
}
}
