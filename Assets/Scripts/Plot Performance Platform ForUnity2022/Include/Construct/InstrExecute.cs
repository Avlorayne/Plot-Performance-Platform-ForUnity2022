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

    public void Init_Pack(InstrParam param)
    {
        Param ??= param;
        if (Param != null)
            ExState = ExState.Ready;

        Init();
    }

    public void Execute_Pack()
    {
        MarkExecuting();
        Execute();
        StartCoroutine(CoExecute_Pack());
    }

    private IEnumerator CoExecute_Pack()
    {
        yield return StartCoroutine(CoExecute());
        MarkCompleted();
    }

    public void Interrupt_Pcak()
    {
        if (!Param.IsCanBeSkipped)
        {
            Debug.Log($"[TypeDialogue.Interrupt] Cannot skip this dialogue");
            return;
        }
        // 停止协程
        StopAllCoroutines();
        Interrupt();
        MarkCompleted();
    }

    public void End_Pack()
    {
        End();
        ExState = ExState.End;
        Debug.Log($"[TypeDialogue.End]");
        if (Param.IsRelese)  ReleaseExecutor();
    }

    #region Executor
    /// Init will be auto called in Init_Pack,so it's not necessary to call Init in this part.
    protected abstract void Init();
    /// Execute will be called before CoExecute if there exists contents here.
    protected abstract void Execute();

    /// CoExecute can be ignored if not needed, or auto called in Execute_Pack.
    protected virtual IEnumerator CoExecute()
    {
        yield return null;
    }
    /// When executing this Instr, Interrupt it.
    protected abstract void Interrupt();
    /// When this Instr is completed, End this.
    protected abstract void End();
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
            Debug.Log($"[InstrExecute.OnCompleted] {ExState}]");
            OnCompleted?.Invoke();
        }
    }

    protected void MarkExecuting()
    {
        if (ExState == ExState.Ready)
        {
            ExState = ExState.Executing;
            Debug.Log($"[InstrExecute.OnExecuting] {ExState}");
            OnExecuting?.Invoke();
        }
    }
    #endregion
}

}
