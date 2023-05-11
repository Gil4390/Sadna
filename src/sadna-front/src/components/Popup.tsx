import React, { useEffect, useState } from "react";
import './Popup.css';
interface PopupProps {
  notification: string;
    onClose: () => void;
  }
  
  const Popup: React.FC<PopupProps> = ({ notification, onClose }) => {
    const [showPopup, setShowPopup] = useState(true);
  
    const handleClosePopup = () => {
      setShowPopup(false);
      onClose();
    };
    
    return (
      showPopup&&  <div className="popup-overlay">
        <div className="popup">
          <div className="popup-content">
            <div>
            <button className="popup-close-button" onClick={handleClosePopup}>
              X
            </button>
            <div className="popup-header">Note! You have received a new message: </div>
           
            <div className="popup-message">{notification}</div>
            </div>
          </div>
        </div>
      </div>
    );
  };
  
  export default Popup;