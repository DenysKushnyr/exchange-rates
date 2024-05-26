import React, {useEffect, useState} from 'react';
import {blue, red} from "@ant-design/colors";
import "./MainPage.css";
import {Input, Table} from "antd";
import BackendService from "../services/BackendService.js";
import currencyLookup from "../helpers/CurrencyLookup.js";
import {Link} from "react-router-dom";
import {useSubsStore} from "../../stores/subsStore.js";

const MainPage = () => {
    const [rates, setRates] = useState([]);
    const [filtered, setFiltered] = useState([]);
    const {currencies} = useSubsStore();
    const columns = [
        {
            title: 'Код валюти',
            dataIndex: 'currencyText',
            key: 'currencyText',
            render: (text) => <Link to={`currency/${text}`}>
                {currencies.includes(text) ?
                text + " 🔔" : text}
            </Link>,
        },
        {
            title: 'НБУ',
            dataIndex: 'nbuRate',
            key: 'nbuRate',
        },
        {
            title: 'Монобанк',
            dataIndex: 'monoRate',
            key: 'monoRate',
        },
        {
            title: 'ПриватБанк',
            dataIndex: 'privatRate',
            key: 'privatRate',
        }
    ];

    useEffect(() => {
        BackendService.getAllRates().then(rates => {
                if (rates !== null) {
                    const userCurrenciesRates = rates.filter(item => 
                        currencies.includes(currencyLookup[item.currencyCode]));
                    const topCurrencies = rates.filter(item =>
                        !currencies.includes(currencyLookup[item.currencyCode]) && (    
                        item.currencyCode === 840 || item.currencyCode === 978));
                    const otherCurrencies = rates.filter(item => 
                        !currencies.includes(currencyLookup[item.currencyCode]) && 
                        item.currencyCode !== 840 && 
                        item.currencyCode !== 978);
                    const allCurrencies = userCurrenciesRates.concat(topCurrencies, otherCurrencies);
                    const result = allCurrencies.map((item, index) => ({
                        ...item,
                        key: index,
                        nbuRate: item.nbuRate === 0 ? '-' : item.nbuRate.toFixed(5),
                        privatRate: item.privatRate === 0 ? '-' : item.privatRate.toFixed(5),
                        monoRate: item.monoRate === 0 ? '-' : item.monoRate.toFixed(5),
                        currencyText: currencyLookup[item.currencyCode],
                    }));
                    setRates(result);
                    setFiltered(result);
                }
            }
        );

    }, []);

    const onInputChange = (e) => {
        const value = e.target.value.trim().toUpperCase();
        if (value === "") {
            setFiltered(rates);
        } else {
            setFiltered(rates.filter(item => {
                return item.currencyText.includes(value);
            }))
        }
    }

    return (
        <div>
            <h1
                style={{color: blue.primary}}>
                Поточні курси обміну валют
            </h1>

            {rates !== null ?
                <>
                    <Input
                        className={"currency-search-input"}
                        placeholder="Почніть вводити код валюти"
                        addonBefore={"Пошук валюти"}
                        onChange={onInputChange}
                    />
                    <Table dataSource={filtered} columns={columns}/>
                </>
                :
                <h1 style={{color: red.primary}}>Сталася помилка. Спробуйте пізніше</h1>
            }


        </div>
    );
};

export default MainPage;