using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Instruction
{
public abstract class InstrExecute : MonoBehaviour
{
    public InstrParam param { protected get; set; }
    public abstract void Exeute();
}

}
