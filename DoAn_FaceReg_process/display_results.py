# Face Recognition

# Importing the libraries
from PIL import Image
from keras.applications.vgg16 import preprocess_input
import base64
from io import BytesIO
import json
import random
import cv2
from keras.models import load_model
import numpy as np
import os
from matplotlib import pyplot
from matplotlib.patches import Rectangle
from mtcnn.mtcnn import MTCNN
import time
import pyodbc
from keras.preprocessing import image
import tensorflow as tf

for gpu in tf.config.experimental.list_physical_devices('GPU'):
    tf.compat.v2.config.experimental.set_memory_growth(gpu, True)

detectorcnn = MTCNN()
model = load_model('finalmodel.h5')

# Loading the cascades
face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')

# def create(conn, id):
#     cursor = conn.cursor()
#     try:
#         cursor.execute('insert into DIEMDANH_T(MaSV) values('+ id + ');')
#     except Exception as e:
#         print(e)
#     conn.commit()

# conn = pyodbc.connect(
#     "Driver={SQL Server Native Client 11.0};"
#     "Server=DESKTOP-37IIIG3;"
#     "Database=FACE_RECOGNITION;"
#     "Trusted_Connection=yes;"
# )

# ph vs face cascade
#?///////
def face_extractor(img):
    # Function detects faces and returns the cropped face
    # If no face detected, it returns the input image

    faces = face_cascade.detectMultiScale(img, 1.3, 5)

    if faces == ():
        return None

    # Crop all faces found
    for (x,y,w,h) in faces:
        #adding rectangle around your face
        cv2.rectangle(img,(x,y),(x+w,y+h),(0,255,255),2)
        cropped_face = img[y:y+h, x:x+w]

    return cropped_face



#Face Recognition with the webcam
'''Extract test face , resize the image to feed to VGG16(224x224x3) , convert to nd-array, predict'''
arayname=[]

for file in os.listdir('hinhtrain'):
    arayname.append(file)

video_capture = cv2.VideoCapture(0)
while True:
    _, frame = video_capture.read()


    face=face_extractor(frame)
    if type(face) is np.ndarray:
        face = cv2.resize(face, (224, 224))
        im = Image.fromarray(face, 'RGB')

           #Resizing into 128x128 because we trained the model with this image size.
        img_array = np.array(im)

        img_array = np.expand_dims(img_array, axis=0)
        pred = model.predict(img_array)
        print('PREEEEEEEEEEEEEEEEEEEE:',pred)

        name="None matching"

        for i in range(len(arayname)):
            if(pred[0][i])>0.99:
                name=arayname[i]
        cv2.putText(frame,name, (50, 50), cv2.FONT_HERSHEY_COMPLEX, 1, (0,255,0), 2)
    else:
        cv2.putText(frame,"No face found", (50, 50), cv2.FONT_HERSHEY_COMPLEX, 1, (0,255,0), 2)
    cv2.imshow('Video', frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
#///---------------

# while True:
#     ret, frame = video_capture.read()
#     #face=face_extractor(frame)
#     pixels = cv2.cvtColor(frame,cv2.COLOR_RGB2BGR)
#     faces = detectorcnn.detect_faces(pixels)
#     if faces != None:

#         try:
#             for face in faces:
#                 x, y, width, height = face['box']
#                 cropped_face = frame[y:y+height, x:x+width]
#                 face = cv2.resize(cropped_face, (224, 224))
#                 im = Image.fromarray(face, 'RGB')
#                 cv2.rectangle(frame,(x,y),(width+x,height+y),(255,0,0),1)
#                 img_array = np.array(im)
#                 img_array = np.expand_dims(img_array, axis=0)
#                 pred = model.predict(img_array)
#                 print('PREEEEEEEEEEEEEEEEEEEE:',pred)
#                 name="None matching"
#                 for i in range(len(arayname)):
#                     if(pred[0][i])>0.90:
#                         name=arayname[i]
#                         #create(conn, str(i+46))
#                 cv2.putText(frame,name, (x-50, y-30), cv2.FONT_HERSHEY_COMPLEX, 1, (0,255,0), 2)
#         except:
#             cv2.putText(frame,"No face found", (50, 50), cv2.FONT_HERSHEY_COMPLEX, 1, (0,255,0), 2)
#     else:
#         cv2.putText(frame,"No face found", (50, 50), cv2.FONT_HERSHEY_COMPLEX, 1, (0,255,0), 2)
#     cv2.imshow('Video', frame)
    
#     if cv2.waitKey(1) & 0xFF == ord('q'):
#         break
video_capture.release()
cv2.destroyAllWindows()