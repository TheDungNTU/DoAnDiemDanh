from flask import Flask
from flask_cors import CORS, cross_origin
from flask import request
import cv2
import time
from train_model import train

# Khởi tạo Flask Server Backend
app = Flask(__name__)

# Apply Flask CORS
CORS(app)
app.config['CORS_HEADERS'] = 'Content-Type'

# http://127.0.0.1/add
# http://127.0.0.1/minus
# http://127.0.0.1/multi
# http://127.0.0.1/div







@app.route('/train', methods=['POST','GET'] )
@cross_origin(origin='*')
def train_process():
    #s = request.form.get("chuoiinput")
    #s = request.args.get("chuoiinput")
    train()
    return ('Train hoan tat')


# Start Backend
if __name__ == '__main__':
    app.run(host='0.0.0.0', port='6868')
