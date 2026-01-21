# CoCoDoogy
![Image](https://github.com/user-attachments/assets/03417ae4-b922-4f4c-9975-6c50247d507d)

#  CoCoDoogy
CoCoDoogy [기업협약 프로젝트] 

코코두기 플레이 영상 https://youtu.be/UZxtOPqYw3k


코코두기 맵에디터 영상 https://youtu.be/SR_TgejYDHo

코코두기 맵에디터 Git https://github.com/lgw0317/DoogyMapEditor

## 목차

1. 프로젝트 소개
2. 주요 기능
3. 기술 스택 



## 1. 프로젝트 소개

기획팀과 함께 진행한 기업협약 프로젝트로, 소코반 스타일의 모바일 안드로이드 3D 퍼즐게임입니다.

Unity를 활용하여 제작하였습니다.

개발기간 : 2025.10.16 ~ 2025.12.10

| 개발팀 | 이름 | 담당 분야 | 
|:------:|:------:|:------|
| 팀장 | 이강욱 | 개발 전반 관리, 기능간 연결 처리, 메뉴 플로우, DB-클라이언트 통신, 유저 데이터 관리 구현, 맵 에디터 개발 |
| 부팀장 | 김현지 | 인게임 메인 로직 구현 |
| 팀원 | 장우형 | 데이터 테이블(CSV) -> Scriptable Object 파싱 구현, 인게임 데이터 핸들링 구현, 맵 에디터 개발 |
| 팀원 | 김민준 | 로비 시스템 구현, 꾸미기 기능 및 인벤토리 기능 구현 |
| 팀원 | 서수하 | UI 전반 디자인 및 개발, 도감 / 퀘스트 / 상점 기능 내부 로직 구현 |
| 팀원 | 이승호 | 사운드 시스템 구조 설계, 로비 캐릭터 행동 및 상호작용 로직 구현 |

| 기획팀 | 이름 | 담당 분야 | 
|:------:|:------:|:------|
| PM | 정다훈 | 기획 전반 관리, 메인 플로우 기획, 콘텐츠 기획 |
| PD | 공병용 | 레벨 디자인, 메인 로직 기획 |
| 팀원 | 장석환 | 데이터 테이블 작성 |
| 팀원 | 윤진비 | 메인 스토리 기획, 콘텐츠 기획 |




## 2. 주요 기능
   
-----------------------------------------------------------

2.1 익명 로그인/구글 로그인 및 익명-구글계정 연동 기능

관련 스크립트: [FirebaseManager.cs](Assets/_Proj/Scripts/Managers/FirebaseManager.cs)

* Firebase Auth SDK 활용
* Google Signin SDK 활용


2.2 유저 데이터 핸들링, 직렬화 및 저장(Firebase Realtime DB)

관련 스크립트: [UserData.cs](Assets/_Proj/Scripts/Firebase/UserData.cs)

* 직렬화 가능 아키텍쳐 설계, 업로드/다운로드 API 구현
* 부분 저장으로 패킷 최적화


2.3 플레이 가능 영역 재설정 로직 구성

관련 스크립트: [IEdgeColliderHandler.cs](Assets/_Proj/Scripts/Stage/Block/Interfaces/IEdgeColliderHandler.cs)

* IEdgeCollider 인터페이스 구성
* Unity 물리 기능 활용, 통과 가능 여부 능동적 재설정 기능 구현


2.4 친구 기능 내부구현 비동기 처리

관련 스크립트: [FirebaseManager.cs](Assets/_Proj/Scripts/Managers/FirebaseManager.cs)

* DB로부터 데이터 인바운드할 수 있도록 처리


2.5 퀘스트 진행도 누적 기능 구현

관련 스크립트: [QuestManager.cs](Assets/_Proj/Scripts/Quest/QuestManager.cs)

* IQuestBehaviour 인터페이스 구성
* 인터페이스 전용 메서드를 통한 우회 처리로 오류 최소화


2.6 입력 방향 스냅 로직 구성

관련 스크립트: [Joystick.cs](Assets/_Proj/Scripts/Player/Joystick.cs)

* 상하좌우 강한 스냅, 대각선 방향 약한 스냅
* 조작감 개선 처리


2.7 게임 플로우 로직 전반 구성

관련 스크립트: [TitleSceneManager.cs](Assets/_Proj/Scripts/Title/TitleSceneManager.cs)

* 타이틀, 스테이지 씬 플로우 로직 구성
* 메뉴 기능 작동을 통한 데이터 핸들링, 업로드/다운로드 로직 전반 구성


-----------------------------------------------------------

## 3. 기술 스택
   
* C#
* Unity
* Fork + Github(형상 관리)
  
-----------------------------------------------------------
기술파트

* UserData 클래스 및 서브클래스 구조 구현
* FirebaseManager 클래스 비동기 처리 API 구현, 인바운드 데이터 처리 구현
* IEdgeColliderHandler 인터페이스 구현 및 지면 블록의 투명 경계 핸들링
* IQuestBehaviour 인터페이스 구현, QuestManager, IQuestBehaviourExtensions 및 퀘스트 관련 처리 객체 구분, 인터페이스 구현

-----------------------------------------------------------
