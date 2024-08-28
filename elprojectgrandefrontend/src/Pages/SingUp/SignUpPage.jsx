/* eslint-disable react/prop-types */
import {useEffect, useState} from 'react';
import {Link, useNavigate} from 'react-router-dom';
import {toast} from 'react-hot-toast';

export const SignupPage = (props) => {
    const {cookies, setUserLoginCookies} = props;
    const [newUserData, setNewUserData] = useState({});
    const [submittable, setSubmittable] = useState(false);
    const navigate = useNavigate();
    const showSuccessToast = (data) => toast.success(data.message);
    const showErrorToast = (data) => toast.error(data.message);
    useEffect(() => {
        if (
            newUserData.Name &&
            newUserData.userName &&
            newUserData.email &&
            newUserData.password &&
            newUserData.doB
        ) {
            setSubmittable(true);
        }
    }, [newUserData]);

    if (cookies.user) {
        navigate('/profile');
    }

    console.log(submittable);

    const handleSignup = async () => {
        const response = await fetch('/api/User/signup', {
            method: 'POST',
            headers: {
                'Content-type': 'application/json',
            },
            body: JSON.stringify({
                ...newUserData,
                salt: 'aaaa',
                registrationTime: new Date(Date.now()).toISOString()
            }),
        });
        const data = await response.json();
        return data;
    };

    const loginNewUser = async () => {
        const response = await fetch('/api/User/Login', {
            method: 'POST',
            headers: {
                'Content-type': 'application/json',
            },
            body: JSON.stringify({email: newUserData.email, password: newUserData.password}),
        });
        const data = await response.json();
        return data;
    };

    return (
        <div className='relative flex justify-center top-48'>
            <div className='relative  p-4 bg-white border border-gray-200 rounded-lg shadow sm:p-6 md:p-8'>
                <form className='w-80 max-w-screen-lg sm:w-96 m-auto'>
                    <h5 className='text-xl font-medium text-gray-900'>Registration</h5>
                    <br/>
                    <div className='flex flex-col gap-6'>
                        <label className='block text-sm font-medium text-gray-900  -mb-4'>
                            Your Name:
                        </label>
                        <input
                            onChange={(event) => {
                                setNewUserData({...newUserData, Name: event.target.value});
                            }}
                            type='text'
                            required
                            placeholder='Your name'
                            className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 dark:text-white'
                        />
                        <label className='block text-sm font-medium text-gray-900  -mb-4'>
                            Your Username:
                        </label>
                        <input
                            onChange={(event) => {
                                setNewUserData({...newUserData, userName: event.target.value});
                            }}
                            type='text'
                            required
                            placeholder='username'
                            className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 dark:text-white'
                        />
                        <label className='block  text-sm font-medium text-gray-900  -mb-4'>
                            Your Email:
                        </label>
                        <input
                            placeholder='name@mail.com'
                            className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 dark:text-white'
                            onChange={(event) => {
                                setNewUserData({...newUserData, email: event.target.value});
                            }}
                            type='email'
                            required
                        />
                        <label className='block text-sm font-medium text-gray-900  -mb-4'>
                            Password:
                        </label>
                        <input
                            type='password'
                            placeholder='********'
                            className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 dark:text-white'
                            onChange={(event) => {
                                setNewUserData({
                                    ...newUserData,
                                    password: event.target.value,
                                });
                            }}
                            required
                        />
                        <label className='block text-sm font-medium text-gray-900  -mb-4'>
                            Date of Birth:
                        </label>
                        <input
                            onChange={(event) => {
                                setNewUserData({...newUserData, doB: event.target.value});
                            }}
                            type='datetime'
                            required
                            placeholder='yyyy-mm-dd'
                            className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 dark:text-white'
                        />
                    </div>
                    <button
                        onClick={(event) => {
                            event.preventDefault();
                            if (
                                newUserData.password.match(/([a-z?'!0-9])/gi).join('') ===
                                newUserData.password
                            ) {
                                handleSignup(event).then((data) => {
                                    if (data.message) {
                                        showErrorToast(data.message);
                                    } else {
                                        setUserLoginCookies(data);
                                        navigate('/profile');
                                    }
                                })
                            } else {
                                showErrorToast({
                                    message: 'Your password or email is invalid',
                                });
                            }

                        }}
                        className='w-full mt-6 bg-blue-500 hover:bg-blue-600 text-white focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center cursor-pointer'
                        disabled={!submittable}
                    >
                        Sign up
                    </button>
                </form>
                <div className='mt-2'>
          <span className='text-center text-slate-500 font-normal'>
            {' '}
              Already have an account?{' '}
          </span>
                    <Link to='/login' className='font-medium text-blue-700'>
                        Login!
                    </Link>
                </div>
            </div>
        </div>
    );
};