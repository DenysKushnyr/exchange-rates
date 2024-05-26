import React from 'react';
import {Avatar, Button, Layout} from "antd";
import {Link} from "react-router-dom";
import "./PageHeader.css";
import {useAuthStore} from "../../stores/tokenStore.js";
import { UserOutlined } from '@ant-design/icons';
import {useSubsStore} from "../../stores/subsStore.js";

const {Header} = Layout;
const PageHeader = () => {
    const {isAuthenticated, email, logout} = useAuthStore();
    const { clear } = useSubsStore();
    
    const logoutHandler = () => {
        logout();
        clear();
    }

    return (
        <div className={"page-header"}>
            <div className="container">
                <Link className={"page-header-logo"} to={"/"}>ExchangeRates</Link>
                <div className="page-header__end">
                    {isAuthenticated ?
                        <div>
                            <Button onClick={logoutHandler}>
                                Вийти
                            </Button>
                            <span className="page-header__email">
                                {email}
                            </span>
                            <Avatar icon={<UserOutlined />}/>
                        </div>
                        :
                        <div>
                            <Button>
                                <Link to={"/register"}>Зареєструватися</Link>
                            </Button>
                            <Button>
                                <Link to={"/login"}>Увійти</Link>
                            </Button>
                        </div>
                    }
                </div>
            </div>
        </div>
    );
};

export default PageHeader;