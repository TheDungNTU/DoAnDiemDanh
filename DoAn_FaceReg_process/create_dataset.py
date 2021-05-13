
import cv2
import numpy as np
from mtcnn.mtcnn import MTCNN
import time
detectorcnn = MTCNN()
#we will try to detect the face of individuals using the haarcascade_frontalface_default.xml
face_classifier=cv2.CascadeClassifier('haarcascade_frontalface_default.xml')

def face_extractor(img):
    # Function detects faces and returns the cropped face
    # If no face detected, it returns the input image

    # faces = face_classifier.detectMultiScale(img, 1.3, 5)
    # #Crop all faces found
    # cropped_face = None
    # for (x,y,w,h) in faces:
    #     #adding rectangle around your face
    #     #cv2.rectangle(img,(x,y),(x+w,y+h),(0,255,255),2)
    #     cropped_face = img[y:y+h, x:x+w]


    #//////////////
    pixels = cv2.cvtColor(img,cv2.COLOR_RGB2BGR)
    faces = detectorcnn.detect_faces(pixels)

    if faces is ():
        return None
    cropped_face = None
    for face in faces:
        x, y, width, height = face['box']
        #cv2.rectangle(img,(x,y),(width+x,height+y),(255,0,0),1)
        cropped_face = img[y:y+height, x:x+width]
    return cropped_face






cap=cv2.VideoCapture(0,cv2.CAP_DSHOW)
count=0

while True:
    #time.sleep(0.05)
    ret,frame=cap.read()
    '''Extract face , convert to grayscale and save it in out folders'''
    if face_extractor(frame) is not None:
        count+=1
        face=cv2.resize(face_extractor(frame),(224,224))
        #face=cv2.cvtColor(face,cv2.COLOR_BGR2GRAY)
        file_name_path='data/train/huy/huy'+str(count)+'.jpg'
        cv2.imwrite(file_name_path,face)
        cv2.putText(face,str(count),(50,50),cv2.FONT_HERSHEY_COMPLEX,1,(0,0,255),2)
        cv2.imshow('Face Cropper',face)
    else:
        print("Face not found")
        pass
    if cv2.waitKey(1)==13 or count==25:
        break
cap.release()
cv2.destroyAllWindows()
print("Collecting Samples Complete!!")