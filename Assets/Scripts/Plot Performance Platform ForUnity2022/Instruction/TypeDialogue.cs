using System;
using System.Collections;
using System.Linq;
using Plot_Performance_Platform_ForUnity2022.Include.Construct;
using Plot_Performance_Platform_ForUnity2022.src.Allocate;
using Plot_Performance_Platform_ForUnity2022.src.Controller;
using TMPro;
using UnityEditor;
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
        public override bool IsCanBeSkipped { get; set; } = true;

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
        protected override void Execute()
        {

        }

        protected override IEnumerator CoExecute()
        {
            speakerNameText.text = currentDialogue.Name;
            // 逐字显示
            foreach (char c in currentDialogue.Sentence)
            {
                dialogueText.text += c;
                // Debug.Log($"[TypeDialogue.CoExecute] {dialogueText.text}...");
                yield return new WaitForSeconds(textSpeed);
            }
            // Debug.Log($"[TypeDialogue.CoExecute] {dialogueText.text}(Completed)");
        }

        // 中断指令
        protected override void Interrupt()
        {
            dialogueText.text = currentDialogue.Sentence;
            // Debug.Log($"[TypeDialogue.Interrupt_Pcak] {dialogueText.text}");
        }

        // 结束指令
        protected override void End()
        {

        }
        #endregion


    }
    #endregion
}
