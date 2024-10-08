# QuickRanking
Unityでランキング機能を実装することができます  
バックエンドにはPlayFabを利用しています  

![image](https://github.com/user-attachments/assets/59ef0f17-8742-4df9-b9d8-5eaee7ab1990)

## 動作環境
Unity2022.3以上  

※Unity2021.3でも、Playfabとの通信時に以下のエラーが出る場合がありますが、一応動作はしました  
[A Native Collection has not been disposed, resulting in a memory leak. Enable Full StackTraces to get more details.]  
このエラーは無視しても問題ないという意見もあるようなのですが、詳細は未調査です  

## 使用方法
### PlayFabの導入
1.下記の公式ページなどを参考にアカウントを作成～タイトルを作成  
[クイックスタート: ゲーム マネージャー](https://learn.microsoft.com/ja-jp/gaming/playfab/gamemanager/quickstart)  

2.クライアントからのデータ更新を許可  
タイトル管理画面→[Settings]→[API Features]→[Allow client to post player statistics]にチェック→[Save]  

3.プレイヤー名の重複を許可(必要なら)  
[Settings]→[General]→[Allow non-unique player names]にチェック→[Save]

4.LeaderBoardの作成  
[LeaderBoard]→[New LeaderBoard]→ランキング名を入力して[Save]

5.下記の公式ページを参考にUnityプロジェクトにSDKをインポート、タイトル設定を行う  
[クイックスタート: Unity の C# 用の PlayFab クライアント ライブラリ](https://learn.microsoft.com/ja-jp/gaming/playfab/sdks/unity3d/quickstart)  

<br>

### QuickRankingの導入
1.unitypackageのインポート  
GitHubのReleaseからQuickRanking.unitypackageをダウンロード、Unityプロジェクトにインポート  

2.RankingSettingの設定  
[Assets/QuickRanking/Resources/RankingSetting 1.asset]  
・RankingIdにPlayfabで作成したLeaderBoard名を入力  
・必要に応じてその他の項目を設定  
　[RankingName]・・・ランキングパネルにタイトルとして表示されるランキング名  
　[OrderBy]・・・降順/昇順  
　[ScorePrefix]・・・スコア表示時に先頭に加える文字列  
　[ScoreSuffix]・・・スコア表示時に末尾に加える文字列  
　[DecimalDigits]・・・小数桁数  
　※[OrderBy]・[DecimalDigits]はPlayfabとスコアを送受信する際にスコアの加工に使用するので、データの整合性を保つため後からの変更はしないでください  

3.RankingPanelをシーンに配置  
◆LegacyText版を使う場合  
　・(WebGL向けビルドの場合のみ) デフォルトのフォントでは日本語が表示できないため、日本語が表示できるフォントをRankingPanelと同じフォルダのText (Legacy)のFontにアタッチ  
　・[Assets/QuickRanking/UI/Legacy/RankingPanel (Legacy)]をシーンに配置  
 <br>
◆TextMeshPro版を使う場合  
　・使用する日本語フォントのFontAssetを作成し、RankingPanelと同じフォルダのText (TMP)のTextMeshProコンポーネントにアタッチ
　・[Assets/QuickRanking/UI/TMP/RankingPanel (TMP)]をシーンに配置  
<br>

※EventSystemがシーン上にない場合は配置
<br>

### 呼び出し
```
QuickRanking.Facade.CallRanking(スコア);
```

