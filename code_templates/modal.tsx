import React from "react";

type ModalProps = {
  title: string;
  content: string;
  onClose: () => void;
};

const Modal: React.FC<ModalProps> = ({ title, content, onClose }) => (
  <div className="modal">
    <div className="modal-content">
      <h2>{title}</h2>
      <p>{content}</p>
      <button onClick={onClose}>關閉</button>
    </div>
  </div>
);

export default Modal;
