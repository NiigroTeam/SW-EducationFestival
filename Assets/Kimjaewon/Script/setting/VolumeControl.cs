using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    // 인스펙터에 슬라이더와 믹서를 할당해야 합니다.
    public Slider volumeSlider;
    public AudioMixer masterMixer; 
    
    // PlayerPrefs에 저장할 키 이름 (변경 없음)
    private const string VOLUME_KEY = "MasterVolumeSave"; 
    
    private const string MIXER_PARAM = "MasterVolume"; 

    private void Start()
    {
        // masterMixer가 인스펙터에서 할당되었는지 확인
        if (volumeSlider != null && masterMixer != null)
        {
            // 이전에 저장된 볼륨 값 불러오기 (기본값 1.0f)
            float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1.0f);
            
            // 슬라이더 값 설정
            volumeSlider.value = savedVolume;
            
            // 시작 시 볼륨을 믹서에 반영
            SetVolume(savedVolume); 

            // 슬라이더 값이 변경될 때마다 SetVolume 함수를 호출하도록 리스너 추가
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        else
        {
            Debug.LogError("VolumeControl 스크립트에 Slider 또는 AudioMixer가 할당되지 않았습니다.");
        }
    }

    /// <summary>
    /// UI 슬라이더 값(0~1)을 받아서 AudioMixer의 데시벨 값으로 변환하여 설정합니다.
    /// </summary>
    /// <param name="value">슬라이더에서 전달된 선형 볼륨 값 (0.0f ~ 1.0f)</param>
    public void SetVolume(float value)
    {
        // 데시벨 공식: Log10(선형값) * 20. 
        // 0 입력 시 -무한대가 되는 것을 방지하기 위해 최소값을 0.0001f로 설정합니다.
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        
        // 믹서 파라미터에 데시벨 값 반영
        // MIXER_PARAM이 이제 Audio Mixer에 존재하는 이름이어야 합니다.
        masterMixer.SetFloat(MIXER_PARAM, dB);

        // 변경된 볼륨 값을 저장
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();
    }
}