import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import WelcomePage from './Pages/WelcomePage/WelcomePage.jsx'

import {BrowserRouter, Routes, Route} from "react-router-dom"

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
    
    <Routes>

    <Route path="/" element={<WelcomePage />} />
      
    
    </Routes>
    
    
    </BrowserRouter>

  </StrictMode>,
)
