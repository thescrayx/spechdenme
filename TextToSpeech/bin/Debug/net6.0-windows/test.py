import requests, json
import io
import wave
import pyaudio
import time
import sys
from pygame import mixer


class Voicevox:
    def __init__(self,host="127.0.0.1",port=50021):
        self.host = host
        self.port = port

    def speak(self,text=None,speaker=47): # VOICEVOX:ナースロボ＿タイプＴ

        params = (
            ("text", text),
            ("speaker", speaker)  # 音声の種類をInt型で指定
        )

        init_q = requests.post(
            f"http://{self.host}:{self.port}/audio_query",
            params=params
        )

        res = requests.post(
            f"http://{self.host}:{self.port}/synthesis",
            headers={"Content-Type": "application/json"},
            params=params,
            data=json.dumps(init_q.json())
        )

        # メモリ上で展開
        audio = io.BytesIO(res.content)

        with wave.open(audio,'rb') as f:
            # 以下再生用処理
            p = pyaudio.PyAudio()

            def _callback(in_data, frame_count, time_info, status):
                data = f.readframes(frame_count)
                return (data, pyaudio.paContinue)

            stream = p.open(format=p.get_format_from_width(width=f.getsampwidth()),
                            channels=f.getnchannels(),
                            rate=f.getframerate(),
                            output=True,
                            stream_callback=_callback)

            # Voice再生
            stream.start_stream()
            while stream.is_active():
                time.sleep(0.1)

            stream.stop_stream()
            stream.close()
            p.terminate()

            with open("voice.wav", "wb") as f:
                f.write(res.content)

            mixer.init(devicename = 'CABLE Input (VB-Audio Virtual Cable)') # Initialize it with the correct device
            mixer.music.load("voice.wav") # Load the mp3
            mixer.music.play() # Play it

            while mixer.music.get_busy():  # wait for music to finish playing
                time.sleep(1)

            mixer.quit() # Mixer'ı kapatın
            sys.exit() # Uygulamayı kapatın


def main():
    if len(sys.argv) > 1:
        vv = Voicevox()
        vv.speak(text=sys.argv[1])
        text = sys.argv[1]
        print("Received text:", text)
    else:
        print("No text provided.")
        sys.exit()

if __name__ == "__main__":
    main()
