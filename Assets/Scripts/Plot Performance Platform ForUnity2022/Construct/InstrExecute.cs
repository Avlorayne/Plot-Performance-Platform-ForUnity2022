using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Construct
{
public class InstrExecute : MonoBehaviour
{
    public InstrParam Param
    {
        get => Param;
        set
        {
            if (Param != null && Param != value && !Param.IsRelese)
            {
                Debug.LogError($"InstrParam {Param.Name} does not exist Here!");
            }
            else
            {
                Param = value;
            }
        }
    }

    protected void ReleaseExecutor()
    {
        Param = null;
    }

    protected void OccupyExecutor(InstrParam param)
    {
        Param = param;
    }

    public virtual void Execute()
    {

    }

    public virtual void Interrupt()
    {

    }
}

}
