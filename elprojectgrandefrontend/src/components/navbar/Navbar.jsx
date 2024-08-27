
import { Link } from "react-router-dom";
import { useNavigate } from 'react-router-dom';

export default function Navbar() {
    const navigate = useNavigate();
    return (
        <div className="navbar fixed-top navbar-expand-lg navbar-light bg-light">
            <Link className="navbar-brand" to="/public">
                Grande
            </Link>
        </div>
    )
}