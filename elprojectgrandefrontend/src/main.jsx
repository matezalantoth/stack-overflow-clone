import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import {BrowserRouter, Routes, Route} from "react-router-dom"

import WelcomePage from './Pages/Welcome/WelcomePage'
import Navbar from './components/navbar/Navbar'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
        <Navbar />
        <Routes>
            <Route path="/" element={<WelcomePage />} />
        </Routes>
    </BrowserRouter>
  </StrictMode>,
)
