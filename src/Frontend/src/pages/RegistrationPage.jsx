import React, {useState} from 'react';
import {Button, Form, Input, message} from "antd";
import './AuthPages.css';
import BackendService from "../services/BackendService.js";
import {useAuthStore} from "../../stores/tokenStore.js";
import {useNavigate} from "react-router-dom";

const RegistrationPage = () => {
    const navigate = useNavigate();
    const {login} = useAuthStore();
    const [messageApi, contextHolder] = message.useMessage();
    const error = (msg) => {
        messageApi.open({
            type: 'error',
            content: msg,
        });
    };
    const onFinish = (values) => {
        BackendService.Register(values.email, values.password).then(response => {
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
                navigate("/");
            }
        });
    };

    return (
        <div>
            {contextHolder}
            <h1>Реєстрація</h1>
            <Form
                name="registration-form"
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
                    label="Підтвердіть пароль"
                    name="confirmPassword"
                    rules={[
                        {
                            required: true,
                            message: 'Please confirm your password!',
                        },
                        ({getFieldValue}) => ({
                            validator(_, value) {
                                if (!value || getFieldValue('password') === value) {
                                    return Promise.resolve();
                                }
                                return Promise.reject(new Error('Паролі не співпадають!'));
                            },
                        }),
                    ]}
                >
                    <Input.Password/>
                </Form.Item>

                <Form.Item
                >
                    <Button type="primary" htmlType="submit">
                        Зареєструватися
                    </Button>
                </Form.Item>
            </Form>
        </div>
    );
};

export default RegistrationPage;