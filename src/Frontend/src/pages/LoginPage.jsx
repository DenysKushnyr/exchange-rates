import React, {useState} from 'react';
import {Button, Form, Input, message} from "antd";
import './AuthPages.css';
import BackendService from "../services/BackendService.js";
import {useAuthStore} from "../../stores/tokenStore.js";
import {useNavigate} from "react-router-dom";
import {useSubsStore} from "../../stores/subsStore.js";
import currencyLookup from "../helpers/CurrencyLookup.js";

const LoginPage = () => {
    const {login} = useAuthStore();
    const {addRange} = useSubsStore();
    const navigate = useNavigate();
    const [messageApi, contextHolder] = message.useMessage();
    const error = (msg) => {
        messageApi.open({
            type: 'error',
            content: msg,
        });
    };
    const onFinish = (values) => {
        BackendService.Login(values.email, values.password).then(response => {
            if (response.token === undefined)
            {
                if (response.message)
                {
                    error(response.message)
                }
                else
                {
                    error("Сталася помилка, спробуйте пізніше")
                }
            } else {
                login(response.token, values.email);
                addRange(response.subs.map(item => currencyLookup[item]));
                navigate("/");
            }
        });
    };

    return (
        <div>
            {contextHolder}
            <h1>Увійти в акаунт</h1>
            <Form
                name="login-form"
                layout={"vertical"}
                style={{
                    maxWidth: 600,
                }}
                initialValues={{
                    remember: true,
                }}
                onFinish={onFinish}
            >
                <Form.Item
                    label="Email"
                    name="email"
                    rules={[
                        {
                            required: true,
                            message: 'Будь ласка, введіть свою електронну пошту!',
                        },
                        {
                            type: 'email',
                            message: 'Будь ласка, введіть дійсну електронну адресу!',
                        },
                    ]}
                >
                    <Input/>
                </Form.Item>

                <Form.Item
                    label="Пароль"
                    name="password"
                    rules={[
                        {
                            required: true,
                            message: 'Будь ласка, введіть пароль!',
                        },
                        {
                            min: 6,
                            message: 'Пароль повинен мати довжину не менше 6 символів!',
                        },
                    ]}
                >
                    <Input.Password/>
                </Form.Item>

                <Form.Item
                >
                    <Button type="primary" htmlType="submit">
                        Увійти
                    </Button>
                </Form.Item>
            </Form>
        </div>
    );
};

export default LoginPage;