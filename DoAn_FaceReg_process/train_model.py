from keras.layers import Input, Lambda, Dense, Flatten
from keras.models import Model
from keras.applications.vgg16 import VGG16
from keras.applications.vgg16 import preprocess_input
from keras.preprocessing import image
from keras.preprocessing.image import ImageDataGenerator
from keras.models import Sequential
import numpy as np
from glob import glob
import matplotlib.pyplot as plt
import os

#tien xu ly du lieu
import pyodbc
import numpy as np
import cv2
import base64
import os
import tensorflow as tf
from matplotlib import pyplot
from matplotlib.patches import Rectangle
from matplotlib.patches import Circle
from mtcnn.mtcnn import MTCNN


for gpu in tf.config.experimental.list_physical_devices('GPU'):
    tf.compat.v2.config.experimental.set_memory_growth(gpu, True)

def train():
  detectorcnn = MTCNN()

  def read(conn):
      print("Read")
      cursor = conn.cursor()
      cursor.execute("select TenHA,MASV,BASE64 from HINHANH")
      for row in cursor:
          img = chuyen_base64_sang_anh(row[2])
          if os.path.exists('./hinhanh/'+f'{row[1]}')==False:
              os.mkdir('./hinhanh/'+f'{row[1]}') 
          path='./hinhanh/'+f'{row[1]}'
          cv2.imwrite(os.path.join(path , f'{row[0]}'+'.jpg'), img)
          cv2.waitKey(0)
      print()
  def chuyen_base64_sang_anh(anh_base64):
      try:
          anh_base64 = np.fromstring(base64.b64decode(anh_base64), dtype=np.uint8)
          anh_base64 = cv2.imdecode(anh_base64, cv2.IMREAD_ANYCOLOR)
      except:
          return "chuyen fail"
      return anh_base64


  conn = pyodbc.connect(
      "Driver={SQL Server Native Client 11.0};"
      "Server=DESKTOP-37IIIG3;"
      "Database=FACE_RECOGNITION;"
      "Trusted_Connection=yes;"
  )
  read(conn)

  for file in os.listdir('hinhanh'):
      if os.path.exists('hinhtrain/'+file)==False:
              os.mkdir('hinhtrain/'+file) 
      if os.path.exists('hinhtest/'+file)==False:
              os.mkdir('hinhtest/'+file) 
      fullpath='hinhanh/'+file
      full_des_train_path='hinhtrain/'+file
      full_des_test_path='hinhtest/'+file
      
      tonganh=len([name for name in os.listdir(fullpath) if os.path.isfile(os.path.join(fullpath, name))])
      print('tong_so_anh',tonganh)
      anhtrain = int(tonganh*3/4)
      print('so_anh_train',anhtrain)
      dem=1
      for img in os.listdir(fullpath):
          pixels = pyplot.imread(fullpath+'/'+img)
          faces = detectorcnn.detect_faces(pixels)
          for face in faces:
              x, y, width, height = face['box']
              crop_img = pixels[y:y+height,x:x+width]
              crop_img = cv2.cvtColor(crop_img,cv2.COLOR_BGR2RGB)  
              crop_img=cv2.resize(crop_img,(224,224))
              if(dem<=anhtrain):     
                  print('train_',dem)
                  cv2.imwrite(os.path.join(full_des_train_path , img), crop_img)
              else:
                  print('val_test',dem)
                  cv2.imwrite(os.path.join(full_des_test_path , img), crop_img)
            
              dem+=1 
              
  #-------------------------------------------

  #re-size all the images to this
  IMAGE_SIZE = [224, 224]

  train_path = 'hinhtrain'
  valid_path = 'hinhtest'

  # add preprocessing layer to the front of VGG
  vgg = VGG16(input_shape=IMAGE_SIZE + [3], weights='imagenet', include_top=False)

  for layer in vgg.layers:
    layer.trainable = False



  folders = glob('hinhtrain/*')
  print(len(folders))

  # No of layers
  x = Flatten()(vgg.output)
  numdense=0
  for file in os.listdir('hinhtrain'):
      numdense+=1
  prediction = Dense(numdense, activation='softmax')(x)

  # create a model object
  model = Model(inputs=vgg.input, outputs=prediction)

  # view the structure of the model
  model.summary()

  # Compile the model
  model.compile(
    loss='categorical_crossentropy',
    optimizer='adam',
    metrics=['accuracy']
  )


  from keras.preprocessing.image import ImageDataGenerator

  train_datagen = ImageDataGenerator(rescale = 1./255,
                                    #  width_shift_range=0.1, 
                                    #  height_shift_range=0.1,
                                    shear_range = 0.2,
                                    zoom_range = 0.2,
                                    horizontal_flip = True)

  test_datagen = ImageDataGenerator(rescale = 1./255)

  training_set = train_datagen.flow_from_directory('hinhtrain',
                                                  target_size = (224, 224),
                                                  batch_size = 32,
                                                  shuffle=True,
                                                  class_mode = 'categorical')

  test_set = test_datagen.flow_from_directory('hinhtest',
                                              target_size = (224, 224),
                                              batch_size = 32,
                                              shuffle=True,
                                              class_mode = 'categorical')
  print('leng is',len(training_set))

  print('leng is t',len(test_set))

  # fit the model
  r = model.fit(
    training_set,
    validation_data=test_set,
    epochs=15,
    steps_per_epoch=len(training_set),
    validation_steps=len(test_set)
  )

  # loss
  # plt.plot(r.history['loss'], label='train loss')
  # plt.plot(r.history['val_loss'], label='val loss')
  # plt.legend()
  # plt.show()
  # plt.savefig('LossVal_loss')

  # accuracies
  # plt.plot(r.history['acc'], label='train acc')
  # plt.plot(r.history['val_acc'], label='val acc')
  # plt.legend()
  # plt.show()
  # plt.savefig('AccVal_acc')

  import tensorflow as tf

  from keras.models import load_model

  ##Saving the model
  model.save('finalmodel.h5')
