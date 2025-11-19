using System;
using System.Collections;
using Plot_Performance_Platform_ForUnity2022.Include.Construct;
using Plot_Performance_Platform_ForUnity2022.src.Allocate;
using TMPro;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Instruction
{
    #region Dialogue
    [Serializable]
    public class Dialogue
    {
        public string Name;
        public string Sentence;

        public Dialogue(string name, string sentence)
        {
            Name = name;
            Sentence = sentence;
        }
    }
    #endregion

    #region Param
    [Serializable]
    public class TypeDialogueParam: InstrParam
    {
        #region These Properties MUSTn't Be Changed!
        public override string Name { get; protected set; } = nameof(TypeDialogueParam);
        protected override string _ExecutorType { get; set; } = nameof(TypeDialogue);
        #endregion

        #region Const Implemented Param
        public override string Description { get; protected set; } = "TypeDialogueParam Description";
        public override bool IsCanCoexist { get; set; } = false;
        #endregion

        #region Main Param

        // 是否在此处释放占用
        public override bool IsRelese { get; set; } = true;
        public override bool IsCanBeSkipped { get; set; } = false;

        public override bool IsBeWaited { get; set; }

        public Dialogue dialogue;

        #endregion

        #region Construction

        public TypeDialogueParam(Dialogue dialogue = null)
        {
            this.dialogue = dialogue;
        }

        #endregion
    }
    #endregion

    #region Executor
    public class TypeDialogue: InstrExecute
    {
        #region Execute Param

        [Header("文本组件")]
        public TextMeshProUGUI speakerNameText;
        public TextMeshProUGUI dialogueText;

        [Header("文本速度")] public float textSpeed = 0.05f;

        Dialogue currentDialogue;

        #endregion

        #region Executor

        protected override void Init()
        {
            speakerNameText.text = "";
            dialogueText.text = "";
            ExState = ExState.Ready;
            currentDialogue = (Param as TypeDialogueParam)?.dialogue;
        }

        // 执行指令
        public override void Execute()
        {
            MarkExecuting();

            StartCoroutine(CoExecute());
        }

        protected override IEnumerator CoExecute()
        {
            #region Execute Part

            // 逐字显示
            foreach (char c in currentDialogue.Sentence)
            {
                dialogueText.text += c;
                Debug.Log($"[TypeDialogue.CoExecute] {dialogueText.text}...");
                yield return new WaitForSeconds(textSpeed);
            }
            Debug.Log($"[TypeDialogue.CoExecute] {dialogueText.text}(Completed)");

            #endregion

            MarkCompleted();
        }

        // 中断指令
        public override void Interrupt()
        {
            if (!Param.IsCanBeSkipped)
            {
                Debug.Log($"[TypeDialogue.Interrupt] Cannot skip this dialogue");
                return;
            }

            // 停止协程
            StopAllCoroutines();

            #region Stop Execute Part
            dialogueText.text = currentDialogue.Sentence;
            Debug.Log($"[TypeDialogue.Interrupt] {dialogueText.text}");
            #endregion

            MarkCompleted();
        }

        // 结束指令
        public override void End()
        {
            #region End Execute Prat


            #endregion
			Debug.Log($"[TypeDialogue.End]");

            if (Param.IsRelese)  ReleaseExecutor();
        }
        #endregion


    }
    #endregion
}
