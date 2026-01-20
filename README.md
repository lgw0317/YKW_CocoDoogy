# CoCoDoogy
![Image](https://github.com/user-attachments/assets/03417ae4-b922-4f4c-9975-6c50247d507d)

#  CoCoDoogy
CoCoDoogy [기업협약 프로젝트] 

코코두기 플레이 영상 https://youtu.be/UZxtOPqYw3k

코코두기 맵에디터 영상 https://youtu.be/SR_TgejYDHo

코코두기 맵에디터 Git https://github.com/lgw0317/DoogyMapEditor

## 1. 코코두기 맵 에디터 소개

기업협약 프로젝트이며, 소코반 스타일의 모바일 안드로이드 3D 퍼즐게임입니다.

Unity를 활용하여 제작하였습니다.

개발기간 : 2025.10.16 ~ 2025.12.10

## 2. 주요 기능
   
-----------------------------------------------------------
담당 Features

2.1 블록 JSON 직렬화, 저장 기능(Firebase Realtime DB)
* JSON 직렬화하여 DB 업로드 및 다운로드

2.2 편집 컨텍스트 기록 및 되돌리기/복원 기능
* ICommandable 인터페이스, Command 구체 클래스 => 편집 기능을 커맨드화하여 작업 내용 누적
* Undo/Redo 기능 구현

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
