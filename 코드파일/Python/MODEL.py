import os
import numpy as np
from PIL import Image
import random
from tensorflow import keras
import cv2
from tensorflow.keras.preprocessing.image import ImageDataGenerator

class DataReader():
    def __init__(self):
        self.label = self.read_label()

        self.train_X = []
        self.train_Y = []
        self.test_X = []
        self.test_Y = []

        self.read_images()

    def read_label(self):
        print("Reading Label...")
        file = open("data/artist.csv")
        labels = []
        file.readline()
        for line in file:
            splt = line.split(",")
            art = splt[1]
            labels.append(art)

        return labels

    def read_images(self):
        data = []
        print("Reading Data...")
        classes = os.listdir("data/artist/")
        for i, cls in enumerate(classes):
            print("Opening " + cls + "/")
            for el in os.listdir("data/art/" + cls):
                img = Image.open("data/art/" + cls + "/" + el)
                img = img.resize((224, 224))
                img_array = np.asarray(img)

                if img_array.ndim == 2:
                    img_array = np.stack([img_array] * 3, axis=-1)
                elif img_array.shape[2] != 3:
                    img_array = cv2.cvtColor(img_array, cv2.COLOR_BGR2RGB)

                data.append((img_array, i))
                img.close()

        random.shuffle(data)

        for i in range(len(data)):
            if i < 0.8*len(data):
                self.train_X.append(data[i][0])
                self.train_Y.append(data[i][1])
            else:
                self.test_X.append(data[i][0])
                self.test_Y.append(data[i][1])

        # 학습용, 테스트용 이미지 데이터를 넘파이 배열로 변환하고 0-1 사이 값으로 정규화
        self.train_X = np.asarray(self.train_X) / 255.0
        self.train_Y = np.asarray(self.train_Y)
        self.test_X = np.asarray(self.test_X) / 255.0
        self.test_Y = np.asarray(self.test_Y)

        # 데이터 읽기가 완료되었습니다.
        # 읽어온 데이터의 정보를 출력합니다.
        print("\n\nData Read Done!")
        print("Training X Size : " + str(self.train_X.shape))
        print("Training Y Size : " + str(self.train_Y.shape))
        print("Test X Size : " + str(self.test_X.shape))
        print("Test Y Size : " + str(self.test_Y.shape) + '\n\n')


EPOCHS = 60

dr = DataReader()

datagen = ImageDataGenerator(horizontal_flip=True, width_shift_range=0.3, height_shift_range=0.3,
                                   rotation_range=50, shear_range=0.3, zoom_range=0.3, vertical_flip=True,
                                   fill_mode='nearest')

train_datagen = datagen.flow(dr.train_X, dr.train_Y, batch_size=64)

model = keras.Sequential([
                        keras.layers.Conv2D(32, (3, 3), activation='relu', input_shape=(224, 224, 3),strides=1),
                        keras.layers.MaxPooling2D((2, 2)),
                        keras.layers.Conv2D(64, (3, 3), activation='relu'),
                        keras.layers.MaxPooling2D((2, 2)),
                        keras.layers.Conv2D(64, (3, 3), activation='relu'),
                        keras.layers.MaxPooling2D((2, 2)),
                        keras.layers.Conv2D(128, (3, 3), activation='relu'),
                        keras.layers.MaxPooling2D((2, 2)),
                        keras.layers.Conv2D(128, (3, 3), activation='relu'),
                        keras.layers.Flatten(),
                        keras.layers.Dense(256, activation='relu'),
                        keras.layers.Dense(len(dr.label), activation="softmax")])

model.compile(optimizer='adam', metrics=['accuracy'],
              loss='sparse_categorical_crossentropy')

early_stop = keras.callbacks.EarlyStopping(monitor='val_loss', patience=10)
model_checkpoint = keras.callbacks.ModelCheckpoint('best_model.h5', monitor='val_loss', save_best_only=True)

history = model.fit(train_datagen, epochs=EPOCHS,
                    validation_data=(dr.test_X, dr.test_Y),
                    callbacks=[model_checkpoint])

model.save('artist.h5')