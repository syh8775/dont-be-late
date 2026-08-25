# Don't Be Late (지각은 안돼)

> 제한 시간 안에 장애물을 피하고 아이템을 활용해 학교에 도착하는 2D 자동 달리기 플랫포머입니다.

<p align="center">
  <img src="Assets/Resources/Images/UI/MainMenuBackground.png" alt="지각은 안돼 메인 메뉴" width="100%">
</p>

## 프로젝트 개요

| 구분 | 내용 |
| --- | --- |
| 장르 | 2D 자동 달리기 플랫포머 |
| 개발 형태 | 개인 프로젝트 |
| 엔진 | Unity 6000.3.13f1 |
| 언어 | C# |
| 플레이 구조 | 3개 스테이지 연속 진행 또는 스테이지 선택 |

## 스테이지

<p align="center">
  <img src="Assets/Resources/Backgrounds/BG_HomeStart_01.png" alt="1스테이지 집 앞" width="32%">
  <img src="Assets/Resources/Backgrounds/BG_Commute_02.png" alt="2스테이지 등굣길" width="32%">
  <img src="Assets/Resources/Backgrounds/BG_SchoolNear_03.png" alt="3스테이지 학교 앞" width="32%">
</p>
<p align="center">
  <sub>1스테이지 집 앞 · 2스테이지 등굣길 · 3스테이지 학교 앞</sub>
</p>

## 게임 흐름

1. 시작 버튼을 누르면 인트로 영상 후 1스테이지가 시작됩니다.
2. 캐릭터는 오른쪽으로 자동 이동하며, 점프로 장애물을 피합니다.
3. 시계는 남은 시간을 늘리고, 음료는 이동 속도를 높이며, 실드는 장애물을 한 번 막아줍니다.
4. 제한 시간 안에 3스테이지를 통과하면 클리어 영상과 결과 화면이 표시됩니다.

## 플레이 요소

<table>
  <tr>
    <td align="center"><img src="Assets/Resources/PlayerFrames/Run/Player_Run_03.png" alt="달리기" width="110"><br><sub>자동 달리기</sub></td>
    <td align="center"><img src="Assets/Resources/ItemSprites/Item_Clock.png" alt="시계 아이템" width="110"><br><sub>시간 추가</sub></td>
    <td align="center"><img src="Assets/Resources/ItemSprites/Item_EnergyDrink.png" alt="음료 아이템" width="110"><br><sub>속도 증가</sub></td>
    <td align="center"><img src="Assets/Resources/ItemSprites/Item_ShieldPlayerBlue.png" alt="실드 아이템" width="110"><br><sub>장애물 방어</sub></td>
    <td align="center"><img src="Assets/Resources/Obstacles/Obstacle_ConeBarricade.png" alt="장애물" width="110"><br><sub>시간 페널티</sub></td>
  </tr>
</table>

## 조작법

| 조작 | 키 |
| --- | --- |
| 점프 / 더블 점프 | `Space` |
| 이동 | 오른쪽 자동 이동 |
| 일시정지·재개·재시작 | 화면 버튼 |

## 핵심 구현

- `GameManager`: 제한 시간, 시작, 클리어, 실패 상태를 관리합니다.
- `PlayerMove`: 자동 이동, 더블 점프, 낙하 리스폰, 속도 증가와 실드 효과를 처리합니다.
- `StageController`: 하나의 씬에서 3개 스테이지 루트를 전환하고 소모된 오브젝트를 초기화합니다.
- `Item`, `Obstacle`, `Goal`: 아이템 효과, 장애물 페널티, 목표 지점 진입을 담당합니다.
- `GameUI`, `SoundManager`: 시작·선택·일시정지·결과 UI와 배경음악·효과음·영상 흐름을 연결합니다.

## 코드 구조

```text
Assets/Scripts/
├── Core/      # 게임 상태, 카메라, 사운드
├── Player/    # 이동, 점프, 애니메이션
├── Stage/     # 스테이지, 아이템, 장애물, 목표
└── UI/        # 메뉴, 타이머, 결과, 영상 흐름
```

## 실행 방법

```bash
git clone https://github.com/syh8775/project_nom.git
cd project_nom
git lfs pull
```

1. Unity Hub에서 클론한 폴더를 엽니다.
2. Unity `6000.3.13f1`로 프로젝트를 실행합니다.
3. `Assets/Scenes/Stage01.unity`를 열고 Play를 누릅니다.

> 이미지·음원·영상 자산은 Git LFS로 관리됩니다.
