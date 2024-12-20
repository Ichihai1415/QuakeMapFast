﻿# 注意
このソフトは現在ベータ版です。以下の**進捗**や**予定**を確認してください。


画像例(旧版含む)
<div display="flex">
  <img src="https://raw.githubusercontent.com/Ichihai1415/QuakeMapFast/release/image/v0.1.0-1.png" width="24%" />
  <img src="https://raw.githubusercontent.com/Ichihai1415/QuakeMapFast/release/image/v0.1.0-2.png" width="24%" />
  <img src="https://raw.githubusercontent.com/Ichihai1415/QuakeMapFast/release/image/v0.1.0-3.png" width="24%" />
  <img src="https://raw.githubusercontent.com/Ichihai1415/QuakeMapFast/release/image/v0.1.0-4.png" width="24%" />
  <img src="https://raw.githubusercontent.com/Ichihai1415/QuakeMapFast/release/image/v0.2.0-1.png" width="24%" />
  <img src="https://raw.githubusercontent.com/Ichihai1415/QuakeMapFast/release/image/v0.2.2-1.png" width="24%" />
</div>

# 仕様
- [P2P地震情報 WebSocket API](https://www.p2pquake.net/develop/json_api_v2/#/P2P%E5%9C%B0%E9%9C%87%E6%83%85%E5%A0%B1%20API/get_ws)を使用して素早く画像を描画します。
> 緊急地震速報 発表検出(554)、各地域ピア数(555)、地震感知情報(561)、地震感知情報 解析結果(9611)は処理をしません。
- 情報受信時に描画・保存・読み上げ送信・テロップ送信ができます。
- 起動直後の情報未受信時は何も表示されません(緑色)。

### 描画
1920x1080の画像を描画します。フォントは[Koruri Regular](https://koruri.github.io/)を使用しています。震度配色は[Kiwi Monitor カラースキーム 第2版](https://kiwimonitor.amebaownd.com/posts/8214427)を改変したものです。

### 保存
JSONデータ・画像を保存できます。画像を公開する場合著作権表示を消さないようにしてください。

### 読み上げ送信
棒読みちゃんにSocket通信で読み上げ指令を送信できます。震度速報では最大震度から3階級まで読み上げします。

### テロップ送信
拙作ソフトの[Telop](https://github.com/Ichihai1415/Telop)にSocket通信で情報を表示させることができます。
背景色・文字色は描画の震度別色と同じで60秒表示です。

### 音声再生
ファイルがある場合のみ再生します。フォルダを作成し音声ファイル(.wav)を配置してください。
> #### 震度速報
> `Sound\scale\[maxInt].wav` `[maxInt]`には震度が入ります(0,1,2,3,4,5-,5+,6-,6+,7)
> #### 緊急地震速報
> `Sound\eew\warn[level].wav` `[level]`には推定最大震度5弱～5強は1、推定最大震度6弱～7は2が入ります

### その他
- コンソールに情報を出力します。
- 投稿用に右クリックメニューで画像やテキストのクリップボードへのコピーができます。設定で自動コピーできます。(されないかも)
- 右クリックメニューでJSONデータの読み込みができます。詳しくはコンソールに表示される内容を確認してください。
- OBS等向けに描画から一定時間以外は緑(0,255,0)にする機能があります。(起動直後も同じ)
- `AppDataPath.txt`に設定の一次保存先のパスがあります。このファイルがあるフォルダには`readme.txt`も作られます。あわせて確認をお勧めします。
- 画像の例が`https://github.com/Ichihai1415/QuakeMapFast/blob/release/image/`にあります。

# 進捗
<!--※"- [x]"は済、"- [ ]"は未です。-->
(情報の詳細は[こちら](https://www.p2pquake.net/develop/json_api_v2/#/P2P%E5%9C%B0%E9%9C%87%E6%83%85%E5%A0%B1%20API/get_history)を確認してください)
- [ ] 地震情報(551)
> - [x] 震度速報
> - [ ] 震源に関する情報
> - [ ] 震度・震源に関する情報
> - [ ] 各地の震度に関する情報
> - [ ] 遠地地震に関する情報
- [ ] 津波予報(552)
- [ ] 津波予報(5520)
- [x] 緊急地震速報(警報)(556)

# 予定
- アイコン
- 起動時に最新情報の取得
- 配色の自由な指定
- テキストを自由に決めれるように

# 更新履歴
## v0.2.4
2024/11/24
- 音声再生機能追加
- EEWの読み上げがされない問題を修正
- 一部処理の調整

## v0.2.3
2024/08/11
- 地図データの更新(釧路地方中南部が降りつぶされない問題を修正)
- 配色の調整

## v0.2.2
2024/03/21
- eewの震度6弱以上地域を赤色に (伴い処理調整)
- 震度x程度とすべきものも震度x程度以上となっていたので修正
- 日本地図データを更新(少し詳しく)　世界地図データを追加(描画はなし)　(そもそもなかったが)世界地図データ表示が間違ってたので修正
- 受信時表示を調整
- READMEの緊急地震速報のサンプル画像を更新

## v0.2.1
2024/01/20
- JSON保存時、解析時エラーとなる問題を修正
- JSONサンプル追加、自動コピー復活

## v0.2.0
2024/01/20
- 処理の大幅な改修(不具合が起きる可能性があります)
- 緊急地震速報(警報)の追加

## v0.1.4
2023/12/03
- API追加によるエラーを修正

## v0.1.3
2023/11/27
- 受信データ保存のサイズが大きくなる問題の修正
- コンソールに一部色を追加
- その他一部処理修正

## v0.1.2
2023/11/25
- 長いメッセージが処理できない問題を修正
- ツイート処理停止
- クリップボードコピーを設定式に

## v0.1.1
2023/08/01
- JSONを読み込んで描画できるように(右クリックメニュー。詳しくはコンソールに表示される内容を確認してください。)

## v0.1.0
2023/07/31
- 震度速報のみ