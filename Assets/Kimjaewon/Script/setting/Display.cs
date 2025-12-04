using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Display : MonoBehaviour
{
    // 유니티 인스펙터에서 연결해야 할 공용 변수들
    public Slider brightnessSlider;
    public Volume volume; // Post-Processing Volume 컴포넌트
    public Image screenOverlay; // Canvas에 올려둔 검은색 이미지 (화면 전체 밝기 조절용)

    // 내부에서 사용할 변수들
    private ColorAdjustments colorAdjustments; // Post-Processing의 Color Adjustments 효과
    private const string BRIGHTNESS_KEY = "MasterBrightness";

    // 슬라이더의 범위 (Start에서 슬라이더의 min/max 값으로 갱신됨)
    private float minValue = -1.5f;
    private float maxValue = 1.5f;

    private void Start()
    {
        // 1. Volume 및 Profile 검사
        if (volume == null || volume.profile == null)
        {
            Debug.LogError("⚠️ Volume 또는 Volume Profile이 연결되지 않았습니다!");
            return;
        }

        // 2. Volume Profile에서 Color Adjustments 효과 가져오기 시도
        if (!volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.LogError("⚠️ VolumeProfile에 'Color Adjustments' 효과가 없습니다!");
            return;
        }

        // 3. 슬라이더 설정 및 저장된 값 로드
        if (brightnessSlider != null)
        {
            // 슬라이더의 범위 설정
            minValue = brightnessSlider.minValue;
            maxValue = brightnessSlider.maxValue;

            // 저장된 밝기 값을 로드 (기본값은 0f)
            float savedBrightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 0f);
            
            // 슬라이더 값 설정 및 이벤트 리스너 등록
            brightnessSlider.value = savedBrightness;
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
            
            // 초기 밝기 적용
            SetBrightness(savedBrightness);
        }
    }

    /// <summary>
    /// 슬라이더 값(value)을 받아 씬 밝기와 화면 오버레이 밝기를 조절하고 값을 저장합니다.
    /// </summary>
    /// <param name="value">슬라이더의 현재 값 (일반적으로 -1.5f ~ 1.5f 사이)</param>
    public void SetBrightness(float value)
    {
        // 1️⃣ 씬 (배경) 밝기 조절 (Post-Processing Volume의 Post Exposure 사용)
        if (colorAdjustments != null)
            colorAdjustments.postExposure.value = value;

        // 2️⃣ UI 포함 전체 밝기(검은 오버레이) 조절
        if (screenOverlay != null)
        {
            // value를 슬라이더 범위 내의 비율(0~1)로 변환
            float t = Mathf.InverseLerp(minValue, maxValue, value);
            
            // t=0 (최소 밝기)일 때 80% 어둡게 (Alpha 0.8f), t=1 (최대 밝기)일 때 0% 어둡게 (Alpha 0f)
            float overlayAlpha = Mathf.Lerp(0.8f, 0f, t); 
            
            // 검은색 오버레이 이미지의 투명도 설정
            screenOverlay.color = new Color(0f, 0f, 0f, overlayAlpha);
        }

        // 3️⃣ 밝기 값 저장
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, value);
        PlayerPrefs.Save();
    }
}