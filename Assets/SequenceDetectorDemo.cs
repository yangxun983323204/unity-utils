using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using YX;
using Detector = YX.SequenceDetector.DetectorProxy;
using DetectState = YX.SequenceDetector.DetectState;

// 该示例演示VR左右手柄交替按扳机键共20下后，显示Target
// 需要包：Input System 、 XR Interaction ToolKit
public class SequenceDetectorDemo : MonoBehaviour
{
    public GameObject Target;

    SequenceDetector _detector = new SequenceDetector();

    InputAction _actionLeftSecBtn;
    InputAction _actionRightSecBtn;

    // Start is called before the first frame update
    void Start()
    {
        _actionLeftSecBtn = new InputAction(binding: "<XRController>{LeftHand}/triggerPressed");
        _actionLeftSecBtn.Enable();
        _actionRightSecBtn = new InputAction(binding: "<XRController>{RightHand}/triggerPressed");
        _actionRightSecBtn.Enable();

        for (int i = 0; i < 10; i++)
        {
            _detector.PushDetector(new Detector()
            {
                Timeout = 5,
                onUpdate = (pre, delta) =>
                {
                    bool down = false;
                    if (_actionRightSecBtn.WasPressedThisFrame())
                        return DetectState.Fail;

                    down = _actionLeftSecBtn.WasPressedThisFrame();
                    return down == true ?
                        DetectState.Success :
                        DetectState.Wait;
                }
            });

            _detector.PushDetector(new Detector()
            {
                Timeout = 5,
                onUpdate = (pre, delta) =>
                {
                    bool down = false;
                    if (_actionLeftSecBtn.WasPressedThisFrame())
                        return DetectState.Fail;

                    down = _actionRightSecBtn.WasPressedThisFrame();
                    return down == true ?
                        DetectState.Success :
                        DetectState.Wait;
                }
            });
        }
        

        _detector.Repeat = false;
        _detector.onSuccess.AddListener(() => {
            Debug.Log("指令成功!");
            Target.SetActive(true);
            _actionLeftSecBtn.Disable();
            _actionRightSecBtn.Disable();
        });
        _detector.onDetecting.AddListener((i,total) => {
            Debug.LogFormat("指令读取中：{0}/{1}",i,total);
        });
        _detector.onFail.AddListener(() => {
            Debug.Log("指令重置!");
        });
        _detector.Start();
    }

    // Update is called once per frame
    void Update()
    {
        _detector.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        _actionLeftSecBtn.Disable(); 
        _actionRightSecBtn.Disable();
    }
}
