import numpy as np
from tensorflow import keras
import cv2
import socket
import threading
import json
from PIL import Image
import io


def readlabel():
    print("Reading Label...")
    with open("data/artist.csv", 'r', encoding='utf-8') as file:
        labels = []
        file.readline()  # 첫번째 헤더는 건너뜀
        for line in file:
            splt = line.strip().split(",")
            art = splt[1]
            labels.append(art)
    return labels

def predict_painter(labels, image):

    model = keras.models.load_model('model/artist01.h5')
    data = np.frombuffer(image, dtype=np.uint8)
    frame = cv2.imdecode(data, 1)
    frame = cv2.resize(frame, (224, 224))
    frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    test = frame[np.newaxis, :, :, :]

    predict = model.predict(test)
    predict_number = np.argmax(predict)

    print(str(predict))
    print("Predicted label:", labels[predict_number])
    painter = labels[predict_number]

    return painter

def recvall(sock, length):
    buf = b'' # 빈바이트 문자 배열 표시
    while length:
        data = sock.recv(length)
        if not data:
            raise EOFError('소켓 오류')
        buf += data
        length -= len(data)
    return buf


def handler(client_socket, addr):
    print('Connected by', addr)
    label = readlabel()

    try:
        data = client_socket.recv(4)
        if not data:
            print("클라이언트 연결 끊김: ", addr)
        else:
            length = int.from_bytes(data, byteorder='big')
            imgdata = recvall(client_socket, length)
            image = Image.open(io.BytesIO(imgdata))
            buffer = io.BytesIO()
            image.save(buffer, format='JPEG')
            image_bytes = buffer.getvalue()

            painter = predict_painter(label, image_bytes)
            sendmsg = {"Value":painter}

            jsonsend = json.dumps(sendmsg)
            print("보낸 메세지 :", jsonsend)
            client_socket.sendall(jsonsend.encode())

    except json.JSONDecodeError:
        print("JSON 디코딩 오류 발생")
    except ConnectionResetError:
        print("클라이언트 연결 끊김: ", addr)
    except Exception as e:
        print("예기치 않은 오류 발생: ", e)
    finally:
        client_socket.close()
        print("클라가 연결 끊음")

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
server_socket.bind(('', 5000))
server_socket.listen()

print("서버 열림!")

try:
    while True:
        client_socket, addr = server_socket.accept()
        thr = threading.Thread(target=handler, args=(client_socket, addr))
        thr.start()

except Exception as e:
    print("Server exception:", e)
finally:
    server_socket.close()