import {useEffect, useState} from "react";
import {useCookies} from "react-cookie";
import {useNavigate} from "react-router-dom";
import Verify from "../../components/Verify/Verify.jsx";

export default function UpdateProfile ({setUserLoginCookies})
{
    const [userDetails, setUserDetails] = useState({ email: '', password: '' });
    const [statusMessage, setStatusMessage] = useState('');
    const [cookies] = useCookies(['user'])
    const navigate = useNavigate();
    
    const[verified, setVerified] = useState(false)
    
    
    
    
    const handleUpdate = async (e) => {
    e.preventDefault();
    
    const updateProfileRequest = {
        email: userDetails.email,
        password: userDetails.password,
    };
    
    try {
        const response = await fetch(`http://localhost:5212/Users/update-profile`,{
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': "Bearer " + cookies.user
            },
            body: JSON.stringify(updateProfileRequest),
        });
        if (response.ok) {
            setStatusMessage('Profile updated Successfully')
        } else {
            const errorData = await response.json();
            setStatusMessage(`Error: ${errorData.message}`);
        }
    } catch (error) {
        console.error(error);
        setStatusMessage('An error occurred. Please try again.');
    }
    
    
};
    

    return verified ? (
        
        <div className="form-div">
            <form className='space-y-6' action='#'>
                <div>
                    <label className='block mb-2 text-sm font-medium text-gray-900'>
                        Your new email (Leave blank if you dont want to change)
                    </label>
                    <input
                        onChange={(event) => {
                            setUserDetails({...userDetails, email: event.target.value});
                        }}
                        type='email'
                        className='bg-gray-50 border border-gray-300  text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 text-white'
                        placeholder='name@company.com'
                        defaultValue={userDetails.email} 
                        required
                    />
                </div>
                <div>
                    <label className='block mb-2 text-sm font-medium text-gray-900 '>
                        Your new password
                    </label>
                    <input
                        onChange={(event) => {
                            setUserDetails({
                                ...userDetails,
                                password: event.target.value,
                            });
                        }}
                        type='password'
                        placeholder='••••••••'
                        className='bg-gray-50 border border-gray-300 text-white text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 '
                        required
                    />
                </div>
                <button
                    onClick={async (event) => {
                       await handleUpdate(event)
                        navigate('/profile')
                    }
                    }
                        
                    type='submit'
                    className='w-full text-white bg-blue-500 hover:bg-blue-600 focus:ring-4 focus:outline-nonefont-medium rounded-lg text-sm px-5 py-2.5 text-center'
                    
                        
                >
                    Save changes
                </button>
            </form>
        </div>
    ) :
        <Verify setDetails={setUserDetails}
        setVerified={setVerified}/>
}