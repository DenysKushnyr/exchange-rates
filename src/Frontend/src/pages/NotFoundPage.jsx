import React from 'react';
import {Button} from "antd";
import {Link} from "react-router-dom";

const NotFoundPage = () => {
    return (
        <div style={{textAlign: "center"}}>
            <h1 style={{textAlign: "center"}}>Запитуваної сторінки не існує</h1>
            
            <Button><Link to={"/"}>На головну</Link></Button>
        </div>
    );
};

export default NotFoundPage;