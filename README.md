TechDays2014 Azure WebJobs 데모 소스
=============================

#이 프로그램은
 - [Microsoft TechDays 2014](http://www.microsoft.com/ko-kr/techdayskorea2014/default.aspx) 세션 중 하나인 "글로벌 웹사이트 구축을 꿈꾸는 당신에게…"에서 소개된 [Azure WebJobs](http://azure.microsoft.com/ko-kr/documentation/articles/web-sites-create-web-jobs/) 데모 소스입니다.
 - 발표: 2014년 9월 24일, 인터컨티넨털호텔.
 - 슬라이드: http://www.slideshare.net/youngjaekim58/20140923-tech-daysazurewebsites

#시나리오
 1. [Youtube](http://www.youtube.com) 동영상링크를 공유하는 게시판 서비스가 있다고 가정한다. 이 때, 링크의 썸네일을 보여주고자 한다.
 1. 가상의 게시판 서비스는 입력된 (1) 동영상 링크와 (2) 저장할 썸네일 이미지 파일을 Json형태로 Queue로 전송한다.
 1. Queue에 입력된 정보에 대하여
   - Youtube 링크가 1개인 경우, 그 영상의 썸네일 이미지를 받아서 `BlobFilename`에 지정된 이름으로 이미지 파일을 저장한다.
   - Youtube 링크가 2개인 경우, 두 동영상의 썸네일 이미지를 받아서 **절반씩 합성**한 후 `BlobFilename`에 지정된 이름으로 이미지 파일을 저장한다.
 1. Queue에 입력된 Youtube 링크가 없는 경우, `error`라고 적힌 텍스트파일을 생성한다.

#설치 및 실행방법

 1. 서비스 개설
   1. [Azure](http://www.azure.com) WebSites 생성
   2. Azure Storage 생성
 2. 설치
   1. Azure WebSites에 git deployment 설정을 한 후 본 솔루션를 Push한다. [방법](http://azure.microsoft.com/en-us/documentation/articles/web-sites-publish-source-control/)
   2. 또는, Visual Studio에서 본 프로젝트에 우클륵한 후 "Publish as Azure WebJobs..."를 선택한다.
   3. Azure WebSites > "웹작업(WebJobs)" 메뉴에 하나의 작업이 생성되었는지 확인.
 3. ConnectionString 설정
   1. Azure Storage의 엑세스키 관리를 클릭하여 키를 복사(primary, secondary 상관없음).
   2. Azure WebSites > Configuration > ConnectionString에서 이름 `AzureWebJobsDashboard`를 생성한 후 값 `DefaultEndpointsProtocol=https;AccountName=tagboa;AccountKey=[STORAGE_ACCESS_KEY]`형태로 키를 입력. `[STORAGE_ACCESS_KEY]`에 앞서 Storage메뉴에서 복사한 액세스키를 입력한다. 세번째 콤보박스는 `사용자 지정`으로 선택함.
   3. `AzureWebJobsStorage`의 이름으로 2번과 동일한 값을 입력.
   4. 저장한다.
 3. 테스트
   1. Storage에 `webjobsqueue` 이름의 Queue를 생성한 후, `{"Links":["http://www.youtube.com/watch?v=h52LpQ1FBm8"],"BlobFilename":"single.jpg"}`를 입력.
   2. 잠시 후 Storage의 Blob 중 `techdays`라는 컨테이너에 `single.jpg`라는 이름의 파일이 생성됨. 다운받아서 해당 Youtube 링크의 썸네일이 성공적으로 생성되었는지 확인한다.
   1. Storage에 `webjobsqueue` 이름의 Queue를 생성한 후, `{"Links":["http://www.youtube.com/watch?v=h52LpQ1FBm8","http://www.youtube.com/watch?v=ZXK5a6IcjLE"],"BlobFilename":"merge.jpg"}`를 입력.
   2. 마찬가지로 잠시 후 Storage의 Blob 중 `techdays`라는 컨테이너에 `merge.jpg`라는 이름의 파일이 생성됨. 다운받아서 두 Youtube 링크의 썸네일 이미지가 반반씩 합성되어있는지 확인한다.
    
#개발환경
 - 언어: C#
 - Visual Studio 2013 Ultimate ENG
 - Windows 8.1 KOR Professional
