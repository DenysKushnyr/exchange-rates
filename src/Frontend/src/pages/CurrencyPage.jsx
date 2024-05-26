import React, {useEffect, useState} from 'react';
import {Link, useLocation} from "react-router-dom";
import {Line} from '@ant-design/charts';
import BackendService from "../services/BackendService.js";
import {Button, Form, message, Popover, Radio, Select, TimePicker} from 'antd';
import './CurrencyPage.css';
import {blue, red} from "@ant-design/colors";
import dayjs from "dayjs";
import {useSubsStore} from "../../stores/subsStore.js";
import {useAuthStore} from "../../stores/tokenStore.js";

const CurrencyPage = () => {
    const currency = useLocation().pathname.split("/").pop();

    const [currencyHistory, setCurrencyHistory] = useState([{rate: 26.4677, date: '23.05.2024'}]);
    const [chartButtonValue, setChartButtonValue] = useState('1m');
    const [currentRate, setCurrentRate] = useState(0);
    const [showSubOptions, setShowSubOptions] = useState(false);
    const [messageApi, contextHolder] = message.useMessage();
    const {currencies, subscribe, unsubscribe} = useSubsStore();
    const {isAuthenticated, token} = useAuthStore();

    useEffect(() => {
        BackendService.GetCurrencyRateHistory(currency, "1m").then(history => {
                if (history !== null) {
                    setCurrencyHistory(history.map(item => {
                        return {
                            "Ціна": item.rate,
                            date: item.date
                        }

                    }));
                    setCurrentRate(history[history.length - 1].rate)
                }
            }
        );

    }, []);


    const config = {
        data: currencyHistory,
        height: 400,
        xField: 'date',
        yField: 'Ціна',
    };

    const chartButtonsOptions = [
        {label: '1 тиждень', value: '1w'},
        {label: '1 місяць', value: '1m'},
        {label: '1 рік', value: '1y'},
    ];

    const chartButtonsHandler = ({target}) => {
        const value = target.value;

        BackendService.GetCurrencyRateHistory(currency, value).then(history => {
                if (history !== null) {
                    setChartButtonValue(value);
                    setCurrencyHistory(history.map(item => {
                        return {
                            "Ціна": item.rate,
                            date: item.date
                        }

                    }));
                }
            }
        );
    };

    const success = () => {
        messageApi.open({
            type: 'success',
            content: 'Дані успішно збережено',
        });
    };
    const error = (msg) => {
        messageApi.open({
            type: 'error',
            content: msg,
        });
    };

    const onFinish = (values) => {
        // 2024-05-26T15:23:11.043318+03:00
        BackendService.CurrencySubscribe(currency, values.time.format("YYYY-MM-DDTHH:mm:ss"), token);
        subscribe(currency);
        success();
    };

    const unsubscribeHandler = () => {
        BackendService.CurrencyUnsubscribe(currency, token);
        unsubscribe(currency);
        success();
    }

    return (
        <div style={{paddingTop: 30}}>
            {contextHolder}
            {currencyHistory.length > 0 ?
                <div>
                    <div className="subscribe-section">
                        <h1>Графік для валюти {currency}</h1>
                        {currencies.includes(currency) ?
                            <div>
                                <Button danger onClick={unsubscribeHandler}>Відмовитися від розсилки</Button>
                            </div>
                            :
                            <div>
                                {isAuthenticated ?
                                    <Button onClick={() => setShowSubOptions(!showSubOptions)}>Отримувати сповіщення про
                                        курс
                                        🔔</Button>
                                    :
                                    <Popover title="Потрібна авторизація" content={(
                                        <div>
                                            <div>
                                                Для виконання цієї дії потрібно бути авторизованим
                                            </div>
                                            <div>
                                                <Button>
                                                    <Link to={"/login"}>Увійти</Link>
                                                </Button>
                                                <Button>
                                                    <Link to={"/register"}>Зареєструватися</Link>
                                                </Button>
                                            </div>
                                        </div>
                                    )}>
                                        <Button disabled={!isAuthenticated}>Отримувати сповіщення про курс
                                            🔔</Button>
                                    </Popover>
                                }
                                {showSubOptions ?
                                    <div className="sub-options">
                                        <Form
                                            onFinish={onFinish}
                                            layout={"inline"}
                                            initialValues={
                                                {
                                                    frequency: "1d",
                                                    time: dayjs('12:00', "HH:mm")
                                                }
                                            }
                                        >
                                            <Form.Item name="frequency" label="Як часто?"
                                                       required
                                            >
                                                <Select
                                                    style={{width: 180}}
                                                    options={[
                                                        {value: '1d', label: 'Кожний день'},
                                                        {value: '1w', label: 'Кожний тиждень'},
                                                        {value: '1m', label: 'Кожний місяць'},
                                                    ]}
                                                />
                                            </Form.Item>
                                            <Form.Item name="time" label="В який час?"
                                                       required
                                            >
                                                <TimePicker
                                                    format={"HH:mm"}
                                                />
                                            </Form.Item>
                                            <Form.Item>
                                                <Button style={{marginTop: 0}} type="primary" htmlType="submit">
                                                    Зберегти
                                                </Button>
                                            </Form.Item>
                                        </Form>
                                    </div>
                                    :
                                    <></>
                                }
                            </div>
                        }
                    </div>
                    <Line {...config}/>
                    <div className="currency-chart-buttons">
                        <span className={"data-source"}>
                            Дані взяті з сайту НБУ
                        </span>

                        <Radio.Group
                            options={chartButtonsOptions}
                            onChange={chartButtonsHandler}
                            value={chartButtonValue}
                            optionType="button"
                            buttonStyle="solid"
                        />

                        <span className={"current-rate"}>
                            Поточна ціна: {currentRate}
                        </span>
                    </div>
                </div>
                :
                <h1
                    style={{color: red.primary}}>
                    Вибачте, але дані про цю валюту наразі недоступні на сайті НБУ
                </h1>
            }

        </div>
    );
};

export default CurrencyPage;