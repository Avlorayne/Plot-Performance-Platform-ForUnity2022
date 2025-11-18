using System.Collections;
using Plot_Performance_Platform_ForUnity2022.src.Allocate;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Include.Construct
{

public abstract class InstrExecute : MonoBehaviour
{
    protected InstrParam _param = null;
    public InstrParam Param
    {
        get => _param;
        set
        {
            if (_param != null && _param != value && !_param.IsRelese)
            {
                Debug.LogError($"InstrParam {_param.Name} shouldn't be Loaded Here!");
            }
            else
            {
                _param = value;
            }
        }
    }

    public ExState ExState = ExState.Null;


    protected void ReleaseExecutor()
    {
        Param = null;
        ExState = ExState.Null;
    }

    public void Init(InstrParam param)
    {
        Param ??= param;
        if (Param != null)
            ExState = ExState.Ready;

        Init();
    }


    #region Executor

    /// Init will be auto called in Init(InstrParam param),so it's not necessary to call Init in this part.
    protected abstract void Init();

    public abstract void Execute();

    /// CoExecute can be ignored if not needed, or call it in Execute.
    protected virtual IEnumerator CoExecute()
    {
        yield return null;
    }
    public abstract void Interrupt();

    /// Called when this Frame is wholely completed and Transfer to the next Frame.
    public abstract void End();

    #endregion


    #region Delegate And Event
    // 添加事件委托和事件
    public delegate void ExecuteHandler();
    public event ExecuteHandler OnCompleted;
    public event ExecuteHandler OnExecuting;

    // 添加标记完成的方法
    protected void MarkCompleted()
    {
        if (ExState == ExState.Executing)
        {
            ExState = ExState.Completed;
            OnCompleted?.Invoke();
        }
    }

    protected void MarkExecuting()
    {
        if (ExState == ExState.Ready)
        {
            ExState = ExState.Executing;
            OnExecuting?.Invoke();
        }
    }
    #endregion
}

}
