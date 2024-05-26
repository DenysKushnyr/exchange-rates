import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import PageHeader from "./components/PageHeader.jsx";
import {Typography} from "antd";
import {Outlet, Route, Routes} from "react-router-dom";
import MainPage from "./pages/MainPage.jsx";

function App() {

  return (
    <Typography>
        <PageHeader />
        <div className="container">
            <Outlet />
        </div>
    </Typography>
  )
}

export default App
